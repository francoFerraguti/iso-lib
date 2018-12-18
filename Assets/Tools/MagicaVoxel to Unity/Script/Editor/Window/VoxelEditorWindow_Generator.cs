namespace VoxeltoUnity {
	using Moenen.Saving;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;


	// Generator Part
	public partial class VoxelEditorWindow {





		#region --- SUB ---



		private enum GeneratorMode {
			Character = 0,
			Map = 1,
		}



		private enum CharacterOrganGroup {
			Head = 0,
			Body = 1,
			ArmL = 2,
			ArmR = 3,
			LegL = 4,
			LegR = 5,
		}



		#endregion




		#region --- VAR ---



		// Global
		private static readonly string[] MAP_STEP_LABELS = new string[] {
			"{0}/{1} Iterating {2}%",
			"{0}/{1} Generating ground {2}%",
			"{0}/{1} Generating cave {2}%",
			"{0}/{1} Generating water {2}%",
			"{0}/{1} Fixing water {2}%",
		};
		private static readonly string[] CHARACTER_STEP_LABELS = new string[] {
			"{0}/{1} Generating physical Body {2}%",
			"{0}/{1} Generating attachment {2}%",
		};



		// Short Cut
		private bool IsGenerating {
			get {
				return GeneratorEditingRoot && GeneratorEditingRoot.gameObject.activeSelf;
			}
		}



		// Data
		private GeneratorMode CurrentGeneratorMode = GeneratorMode.Character;
		private Core_MapGeneration.Preset MGConfig = new Core_MapGeneration.Preset();
		private Core_CharacterGeneration.Preset CGConfig = new Core_CharacterGeneration.Preset();
		private CharacterOrganGroup CurrentOrganGroup = CharacterOrganGroup.Head;


		// Saving
		private EditorSavingBool PhysicalBodyControllerOpen = new EditorSavingBool("VEditor.PhysicalBodyControllerOpen", true);
		private EditorSavingBool PhysicalBodyTableOpen = new EditorSavingBool("VEditor.PhysicalBodyTableOpen", false);
		private EditorSavingBool AttachmentControllerOpen = new EditorSavingBool("VEditor.AttachmentControllerOpen", false);




		#endregion




		#region --- GUI ---



		private void GeneratorEditingGUI () {
			if (!IsGenerating) { return; }
			switch (CurrentGeneratorMode) {
				case GeneratorMode.Character:
					CharacterGeneratorGUI();
					break;
				case GeneratorMode.Map:
					MapGeneratorGUI();
					break;
			}
		}



		private void GeneratorPanelGUI () {

			const int WIDTH = 158;
			const int HEIGHT = 30;
			const int GAP = 3;

			var buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft) {
				fontSize = 11,
			};
			var dotStyle = new GUIStyle(GUI.skin.label) {
				richText = true,
				alignment = TextAnchor.MiddleLeft,
			};

			Rect rect = new Rect(
				ViewRect.x + ViewRect.width - WIDTH,
				ViewRect.y + ViewRect.height - HEIGHT - GAP,
				WIDTH,
				HEIGHT
			);

			if (GUI.Button(rect, "   Generate", buttonStyle)) {
				Generate();
			}
			rect.y -= HEIGHT + GAP;


			if (GUI.Button(rect, "   Generate And Save", buttonStyle)) {
				Generate();
				SaveData();
			}
			GUI.Label(rect, "  <color=#cc66ff>●</color>", dotStyle);
			rect.y -= HEIGHT + GAP;


			if (GUI.Button(rect, "   Export Preset", buttonStyle)) {
				ExportPreset_Dialog();
			}
			GUI.Label(rect, "  <color=#cccccc>●</color>", dotStyle);
			rect.y -= HEIGHT + GAP;

			if (GUI.Button(rect, "   Import Preset", buttonStyle)) {
				ImportPreset_Dialog();
			}
			GUI.Label(rect, "  <color=#cccccc>●</color>", dotStyle);
			rect.y -= HEIGHT + GAP;
		}



		private void CharacterGeneratorGUI () {
			LayoutV(() => {

				var tintColor = new Color(0.8f, 0.85f, 0.8f);
				const int HEIGHT = 16;
				const int LABEL_WIDTH = 80;
				const int ITEM_WIDTH = 32;
				const int TABLE_LINE_COUNT = 3;
				const int TABLE_GAP = 14;

				bool changed = false;
				bool uiChanged = false;

				GUIRect(0, 1);


				// Physical Body Controller
				uiChanged = AltLayoutF(() => {
					LayoutH(() => {
						PhysicalBodyHumanMapGUI();
						changed = PhysicalBodyControllerContentGUI() || changed;
					});
				}, "Physical Body Controller", PhysicalBodyControllerOpen) || uiChanged;



				// Physical Body Table
				uiChanged = AltLayoutF(() => {

					var textFieldStyle = new GUIStyle(GUI.skin.textField) {
						alignment = TextAnchor.MiddleCenter,
					};

					var oldC = GUI.color;
					GUI.color = tintColor;


					// Title
					LayoutH(() => {

						for (int i = 0; i < TABLE_LINE_COUNT; i++) {

							Space(TABLE_GAP);

							// Organ
							Rect rect = GUIRect(LABEL_WIDTH, HEIGHT);
							rect.height *= 2;
							EditorGUI.LabelField(rect, "Organ", textFieldStyle);

							// Size Pos
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH * 3, HEIGHT), "Size", textFieldStyle);
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH * 3, HEIGHT), "Position", textFieldStyle);

							// Skin
							rect = GUIRect(18, HEIGHT);
							rect.height *= 2;
							EditorGUI.LabelField(rect, "S", textFieldStyle);

						}

					});


					// Sub Title
					LayoutH(() => {
						for (int i = 0; i < TABLE_LINE_COUNT; i++) {

							Space(TABLE_GAP);

							// Organ Space
							GUIRect(LABEL_WIDTH, HEIGHT);

							// Size Pos
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH, HEIGHT), "X", textFieldStyle);
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH, HEIGHT), "Y", textFieldStyle);
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH, HEIGHT), "Z", textFieldStyle);
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH, HEIGHT), "X", textFieldStyle);
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH, HEIGHT), "Y", textFieldStyle);
							EditorGUI.LabelField(GUIRect(ITEM_WIDTH, HEIGHT), "Z", textFieldStyle);

							// Skin Space
							GUIRect(18, HEIGHT);
						}
					});

					GUI.color = oldC;


					// Head
					LayoutH(() => {
						changed = OrganTableItemGUI(CGConfig.ArmU.Left, "Upper Arm L", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.LegU.Left, "Upper Leg L", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.Head, "Head", textFieldStyle) || changed;

					});
					LayoutH(() => {
						changed = OrganTableItemGUI(CGConfig.ArmU.Right, "Upper Arm R", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.LegU.Right, "Upper Leg R", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.Body, "Body", textFieldStyle) || changed;
					});
					LayoutH(() => {
						changed = OrganTableItemGUI(CGConfig.ArmD.Left, "Lower Arm L", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.LegD.Left, "Lower Leg L", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.Neck, "Neck", textFieldStyle) || changed;
					});
					LayoutH(() => {
						changed = OrganTableItemGUI(CGConfig.ArmD.Right, "Lower Arm R", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.LegD.Right, "Lower Leg R", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.Hip, "Hip", textFieldStyle) || changed;

					});
					LayoutH(() => {
						changed = OrganTableItemGUI(CGConfig.Hand.Left, "Hand L", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.Foot.Left, "Foot L", textFieldStyle) || changed;
					});
					LayoutH(() => {
						changed = OrganTableItemGUI(CGConfig.Hand.Right, "Hand R", textFieldStyle) || changed;
						changed = OrganTableItemGUI(CGConfig.Foot.Right, "Foot R", textFieldStyle) || changed;
					});

				}, "Physical Body Data Table", PhysicalBodyTableOpen) || uiChanged;



				// Attachment Controller
				uiChanged = AltLayoutF(() => {




				}, "Attachment Controller", AttachmentControllerOpen) || uiChanged;




				if (changed) {
					CGConfig.FixGenerationValues();
					Save_Generation();
					SetDataDirty();
				} else if (uiChanged) {
					Save_Generation_UI();
				}

			}, false, new GUIStyle(GUI.skin.box) {
				padding = new RectOffset(9, 6, 4, 4),
				margin = new RectOffset(14, 20, 8, 4),
			});
		}



		private void MapGeneratorGUI () {
			LayoutV(() => {

				const int HEIGHT = 16;
				const int LABEL_WIDTH = 82;
				const int GAP_WIDTH = 22;
				const int GAP_HEIGHT = 6;

				bool changed = false;
				Space(GAP_HEIGHT);

				// Size 
				LayoutH(() => {
					changed = IntField(LABEL_WIDTH, HEIGHT, "Map Size", ref MGConfig.SizeX) || changed;
					Space(2);
					changed = IntField(16, HEIGHT, "×", ref MGConfig.SizeY) || changed;
				});
				Space(GAP_HEIGHT);

				LayoutH(() => {
					// Iteration
					changed = IntField(LABEL_WIDTH, HEIGHT, "Iteration", ref MGConfig.Iteration) || changed;
					Space(GAP_WIDTH);
					// Radius
					changed = FloatField(LABEL_WIDTH, HEIGHT, "Iter Radius", ref MGConfig.IterationRadius) || changed;
					Space(GAP_WIDTH);
					// Lerp
					changed = FloatField(LABEL_WIDTH, HEIGHT, "Iter Lerp", ref MGConfig.IterationLerp) || changed;

				});
				Space(GAP_HEIGHT - 1);


				LayoutH(() => {
					// Island
					var newIsland = EditorGUI.ToggleLeft(GUIRect(LABEL_WIDTH - 8, HEIGHT), " Island", MGConfig.Island);
					if (newIsland != MGConfig.Island) {
						MGConfig.Island = newIsland;
						changed = true;
					}
					Space(GAP_WIDTH);

					// Tint
					var newTint = EditorGUI.ToggleLeft(GUIRect(LABEL_WIDTH - 8, HEIGHT), " Tint", MGConfig.Tint);
					if (newTint != MGConfig.Tint) {
						MGConfig.Tint = newTint;
						changed = true;
					}
					Space(GAP_WIDTH);

					// Land
					var newLand = EditorGUI.ToggleLeft(GUIRect(LABEL_WIDTH - 8, HEIGHT), " Land", MGConfig.Land);
					if (newLand != MGConfig.Land) {
						MGConfig.Land = newLand;
						changed = true;
					}
					Space(GAP_WIDTH);

					// Water
					var newWater = EditorGUI.ToggleLeft(GUIRect(LABEL_WIDTH - 8, HEIGHT), " Water", MGConfig.Water);
					if (newWater != MGConfig.Water) {
						MGConfig.Water = newWater;
						changed = true;
					}
					Space(GAP_WIDTH);

					// Cave
					var newCave = EditorGUI.ToggleLeft(GUIRect(LABEL_WIDTH - 8, HEIGHT), " Cave", MGConfig.Cave);
					if (newCave != MGConfig.Cave) {
						MGConfig.Cave = newCave;
						changed = true;
					}
				});
				Space(GAP_HEIGHT - 1);


				LayoutH(() => {
					// Land Height
					changed = MinMaxField(LABEL_WIDTH, 18, 0, HEIGHT, "Land Height", "", " -", ref MGConfig.GroundHeight, MGConfig.Land) || changed;
					Space(GAP_WIDTH);
					// Land Bump
					changed = FloatField(LABEL_WIDTH, HEIGHT, "Land Bump", ref MGConfig.GroundBump, MGConfig.Land) || changed;
				});
				Space(GAP_HEIGHT);


				LayoutH(() => {
					// Water Height
					changed = MinMaxField(LABEL_WIDTH, 18, 0, HEIGHT, "Water Height", "", " -", ref MGConfig.WaterHeight, MGConfig.Water) || changed;
					Space(GAP_WIDTH);
					// Water Bump
					changed = FloatField(LABEL_WIDTH, HEIGHT, "Water Bump", ref MGConfig.WaterBump, MGConfig.Water) || changed;
				});
				Space(GAP_HEIGHT);


				LayoutH(() => {
					// Cave Height
					changed = MinMaxField(LABEL_WIDTH, 18, 0, HEIGHT, "Cave Height", "", " -", ref MGConfig.CaveHeight, MGConfig.Cave) || changed;
					Space(GAP_WIDTH);
					// Cave Bump
					changed = FloatField(LABEL_WIDTH, HEIGHT, "Cave Bump", ref MGConfig.CaveBump, MGConfig.Cave) || changed;
					Space(GAP_WIDTH);
					// Cave Radius 
					changed = FloatField(LABEL_WIDTH, HEIGHT, "Cave Radius", ref MGConfig.CaveRadius, MGConfig.Cave) || changed;
				});
				Space(GAP_HEIGHT);




				// Colors
				Space(GAP_HEIGHT);
				LayoutH(() => {
					// Label
					GUI.Label(GUIRect(LABEL_WIDTH - 8, HEIGHT), "Land");
					// Add Ground Color
					if (Button(24, HEIGHT, "+", MGConfig.GroundColors.Count < Core_MapGeneration.Preset.MAX_COLOR_COUNT, EditorStyles.miniButtonLeft)) {
						MGConfig.GroundColors.Add(MGConfig.GroundColors.Count > 0 ? MGConfig.GroundColors[MGConfig.GroundColors.Count - 1] : Color.grey);
						changed = true;
						Repaint();
					}
					// Ground Color
					for (int i = 0; i < MGConfig.GroundColors.Count; i++) {
						var color = ColorField(0, HEIGHT, "", MGConfig.GroundColors[i]);
						if (color != MGConfig.GroundColors[i]) {
							MGConfig.GroundColors[i] = color;
							changed = true;
						}
					}
					// Remove Ground Color
					if (Button(24, HEIGHT, "-", MGConfig.GroundColors.Count > 0, EditorStyles.miniButtonRight)) {
						MGConfig.GroundColors.RemoveAt(MGConfig.GroundColors.Count - 1);
						changed = true;
						Repaint();
					}
				});
				Space(GAP_HEIGHT);

				LayoutH(() => {
					// Label
					GUI.Label(GUIRect(LABEL_WIDTH - 8, HEIGHT), "Grass");
					// Add Grass Color
					if (Button(24, HEIGHT, "+", MGConfig.GrassColors.Count < Core_MapGeneration.Preset.MAX_COLOR_COUNT, EditorStyles.miniButtonLeft)) {
						MGConfig.GrassColors.Add(MGConfig.GrassColors.Count > 0 ? MGConfig.GrassColors[MGConfig.GrassColors.Count - 1] : Color.green);
						changed = true;
						Repaint();
					}
					// Grass Color
					for (int i = 0; i < MGConfig.GrassColors.Count; i++) {
						var color = ColorField(0, HEIGHT, "", MGConfig.GrassColors[i]);
						if (color != MGConfig.GrassColors[i]) {
							MGConfig.GrassColors[i] = color;
							changed = true;
						}
					}
					// Remove Grass Color
					if (Button(24, HEIGHT, "-", MGConfig.GrassColors.Count > 0, EditorStyles.miniButtonRight)) {
						MGConfig.GrassColors.RemoveAt(MGConfig.GrassColors.Count - 1);
						changed = true;
						Repaint();
					}
				});
				Space(GAP_HEIGHT);

				LayoutH(() => {
					GUI.Label(GUIRect(LABEL_WIDTH - 8, HEIGHT), "Water");
					var color = ColorField(0, HEIGHT, "", MGConfig.WaterColor);
					if (color != MGConfig.WaterColor) {
						MGConfig.WaterColor = color;
						changed = true;
					}
				});
				Space(GAP_HEIGHT);



				// Random Button
				Space(GAP_HEIGHT);
				LayoutH(() => {
					// Random
					if (GUI.Button(GUIRect(LABEL_WIDTH + 30, HEIGHT), "Random", EditorStyles.miniButton)) {
						MGConfig.CreateSeed();
						EditorApplication.delayCall += Generate;
						changed = true;
					}
					Space(GAP_WIDTH);
					// Seed
					string seedLabel = "(Null)";
					if (!string.IsNullOrEmpty(MGConfig.Seeds)) {
						seedLabel = "Seed:" + MGConfig.Seeds.Substring(0, Mathf.Min(16, MGConfig.Seeds.Length)) + "...";
					}
					GUI.Label(GUIRect(220, HEIGHT), seedLabel);
				});
				Space(GAP_HEIGHT);


				if (changed) {
					MGConfig.FixGenerationValues();
					Save_Generation();
					SetDataDirty();
				}

			}, false, new GUIStyle(GUI.skin.box) {
				padding = new RectOffset(9, 6, 4, 4),
				margin = new RectOffset(14, 20, 8, 4),
			});
		}



		private void PhysicalBodyHumanMapGUI () {
			LayoutV(() => {

				var tintColor = new Color(0.8f, 0.85f, 0.8f);
				var disableColor = new Color(0.8f, 0.85f, 0.8f, 0.9f);
				var buttonStyle = EditorStyles.miniButton;
				var rect = new Rect();
				var allRect = GUIRect(90, 120);
				var oldE = GUI.enabled;
				var oldC = GUI.color;

				// Head
				GUI.enabled = CurrentOrganGroup != CharacterOrganGroup.Head;
				GUI.color = GUI.enabled ? tintColor : disableColor;
				rect.width = allRect.width * 0.618f;
				rect.height = allRect.height * 0.309f;
				rect.x = allRect.x + (allRect.width - rect.width) / 2f;
				rect.y = allRect.y + 2;
				if (GUI.Button(rect, "", buttonStyle)) {
					CurrentOrganGroup = CharacterOrganGroup.Head;
				}

				// Body
				GUI.enabled = CurrentOrganGroup != CharacterOrganGroup.Body;
				GUI.color = GUI.enabled ? tintColor : disableColor;
				rect.y += rect.height + 2;
				rect.height = allRect.height * 0.32f;
				rect.width = allRect.width * 0.618f * 0.618f;
				rect.x = allRect.x + (allRect.width - rect.width) / 2f;
				if (GUI.Button(rect, "", buttonStyle)) {
					CurrentOrganGroup = CharacterOrganGroup.Body;
				}

				// Leg
				rect.y += rect.height + 2;
				rect.height = allRect.height * 0.309f;
				rect.width = allRect.width * 0.618f * 0.618f * 0.5f;
				rect.x = allRect.x + (allRect.width - rect.width * 2f) / 2f;
				GUI.enabled = CurrentOrganGroup != CharacterOrganGroup.LegL;
				GUI.color = GUI.enabled ? tintColor : disableColor;
				if (GUI.Button(rect, "", buttonStyle)) {
					CurrentOrganGroup = CharacterOrganGroup.LegL;
				}
				GUI.enabled = CurrentOrganGroup != CharacterOrganGroup.LegR;
				GUI.color = GUI.enabled ? tintColor : disableColor;
				rect.x += rect.width;
				if (GUI.Button(rect, "", buttonStyle)) {
					CurrentOrganGroup = CharacterOrganGroup.LegR;
				}

				// Arm
				rect.height = allRect.height * 0.309f + 20f;
				rect.width = allRect.width * 0.618f * 0.618f * 0.5f;
				rect.x = allRect.x + allRect.width * 0.1f;
				rect.y = allRect.y + allRect.height * 0.309f + 6f;
				GUI.enabled = CurrentOrganGroup != CharacterOrganGroup.ArmL;
				GUI.color = GUI.enabled ? tintColor : disableColor;
				if (GUI.Button(rect, "", buttonStyle)) {
					CurrentOrganGroup = CharacterOrganGroup.ArmL;
				}
				GUI.enabled = CurrentOrganGroup != CharacterOrganGroup.ArmR;
				GUI.color = GUI.enabled ? tintColor : disableColor;
				rect.x = allRect.width - rect.width + 2f * allRect.x - rect.x;
				if (GUI.Button(rect, "", buttonStyle)) {
					CurrentOrganGroup = CharacterOrganGroup.ArmR;
				}


				GUI.enabled = oldE;
				GUI.color = oldC;

			}, true);
		}



		private bool PhysicalBodyControllerContentGUI () {

			const int GAP_WIDTH = 12;

			bool changed = false;

			Space(GAP_WIDTH);

			switch (CurrentOrganGroup) {
				case CharacterOrganGroup.Head:
					changed = OrganControllerItemGUI(CGConfig.Head, "Head") || changed;
					changed = OrganControllerItemGUI(CGConfig.Neck, "Neck") || changed;
					break;
				case CharacterOrganGroup.Body:
					changed = OrganControllerItemGUI(CGConfig.Body, "Body") || changed;
					changed = OrganControllerItemGUI(CGConfig.Hip, "Hip") || changed;
					break;
				case CharacterOrganGroup.ArmL:
					changed = OrganControllerItemGUI(CGConfig.ArmU.Left, "Left Upper Arm", CGConfig.ArmU) || changed;
					changed = OrganControllerItemGUI(CGConfig.ArmD.Left, "Left Lower Arm", CGConfig.ArmD) || changed;
					changed = OrganControllerItemGUI(CGConfig.Hand.Left, "Left Hand", CGConfig.Hand) || changed;
					break;
				case CharacterOrganGroup.ArmR:
					changed = OrganControllerItemGUI(CGConfig.ArmU.Right, "Right Upper Arm", CGConfig.ArmU) || changed;
					changed = OrganControllerItemGUI(CGConfig.ArmD.Right, "Right Lower Arm", CGConfig.ArmD) || changed;
					changed = OrganControllerItemGUI(CGConfig.Hand.Right, "Right Hand", CGConfig.Hand) || changed;
					break;
				case CharacterOrganGroup.LegL:
					changed = OrganControllerItemGUI(CGConfig.LegU.Left, "Left Upper Leg", CGConfig.LegU) || changed;
					changed = OrganControllerItemGUI(CGConfig.LegD.Left, "Left Lower Leg", CGConfig.LegD) || changed;
					changed = OrganControllerItemGUI(CGConfig.Foot.Left, "Left Foot", CGConfig.Foot) || changed;
					break;
				case CharacterOrganGroup.LegR:
					changed = OrganControllerItemGUI(CGConfig.LegU.Right, "Right Upper Leg", CGConfig.LegU) || changed;
					changed = OrganControllerItemGUI(CGConfig.LegD.Right, "Right Lower Leg", CGConfig.LegD) || changed;
					changed = OrganControllerItemGUI(CGConfig.Foot.Right, "Right Foot", CGConfig.Foot) || changed;
					break;
			}

			return changed;
		}



		private bool OrganTableItemGUI (Core_CharacterGeneration.OrganData organ, string name, GUIStyle labelStyle) {

			bool changed = false;

			const int HEIGHT = 18;
			const int LABEL_WIDTH = 80;
			const int ITEM_WIDTH = 32;
			const int TABLE_GAP = 14;

			Space(TABLE_GAP);

			// Name
			var oldC = GUI.color;
			GUI.color = new Color(0.8f, 0.85f, 0.8f);
			EditorGUI.LabelField(GUIRect(LABEL_WIDTH, HEIGHT), name, labelStyle);
			GUI.color = oldC;

			// Size
			changed = IntField(ITEM_WIDTH, HEIGHT, "", ref organ.SizeX, true, labelStyle) || changed;
			changed = IntField(ITEM_WIDTH, HEIGHT, "", ref organ.SizeY, true, labelStyle) || changed;
			changed = IntField(ITEM_WIDTH, HEIGHT, "", ref organ.SizeZ, true, labelStyle) || changed;

			// Pos
			changed = IntField(ITEM_WIDTH, HEIGHT, "", ref organ.X, true, labelStyle) || changed;
			changed = IntField(ITEM_WIDTH, HEIGHT, "", ref organ.Y, true, labelStyle) || changed;
			changed = IntField(ITEM_WIDTH, HEIGHT, "", ref organ.Z, true, labelStyle) || changed;

			// Skin
			var color = ColorField(0, HEIGHT, "", organ.SkinColor);
			if (color != organ.SkinColor) {
				organ.SkinColor = color;
				changed = true;
			}


			return changed;
		}



		private bool OrganControllerItemGUI (Core_CharacterGeneration.OrganData organ, string name, Core_CharacterGeneration.OrganData2 organ2 = null) {

			bool changed = false;

			const int HEIGHT = 22;
			const int LABEL_WIDTH = 52;
			const int LABEL_WIDTH_ALT = 24;
			const int BUTTON_WIDTH = 24;
			const int GAP_HEIGHT = 4;
			const int GAP_WIDTH_ALT = 4;

			LayoutV(() => {

				// Name
				GUI.Label(GUIRect(0, HEIGHT), name);

				// Size
				LayoutH(() => {
					GUI.Label(GUIRect(LABEL_WIDTH, HEIGHT), "Size");
					// X
					changed = IntField(LABEL_WIDTH_ALT, HEIGHT, "", ref organ.SizeX) || changed;
					changed = AddReduceButtons(BUTTON_WIDTH, HEIGHT, ref organ.SizeX) || changed;
					Space(GAP_WIDTH_ALT);
					// Y
					changed = IntField(LABEL_WIDTH_ALT, HEIGHT, "", ref organ.SizeY) || changed;
					changed = AddReduceButtons(BUTTON_WIDTH, HEIGHT, ref organ.SizeY) || changed;
					Space(GAP_WIDTH_ALT);
					// Z
					changed = IntField(LABEL_WIDTH_ALT, HEIGHT, "", ref organ.SizeZ) || changed;
					changed = AddReduceButtons(BUTTON_WIDTH, HEIGHT, ref organ.SizeZ) || changed;
				});

				Space(GAP_HEIGHT);

				// Position
				LayoutH(() => {

					GUI.Label(GUIRect(LABEL_WIDTH, HEIGHT), "Position");
					var oldE = GUI.enabled;
					var oldC = GUI.color;
					var tintColor = new Color(1, 1, 1, 0.618f);

					// X
					GUI.enabled = organ.EnableX;
					GUI.color = GUI.enabled ? oldC : tintColor;
					changed = IntField(LABEL_WIDTH_ALT, HEIGHT, "", ref organ.X, GUI.enabled) || changed;
					changed = AddReduceButtons(BUTTON_WIDTH, HEIGHT, ref organ.X) || changed;
					Space(GAP_WIDTH_ALT);

					// Y
					GUI.enabled = organ.EnableY;
					GUI.color = GUI.enabled ? oldC : tintColor;
					changed = IntField(LABEL_WIDTH_ALT, HEIGHT, "", ref organ.Y, GUI.enabled) || changed;
					changed = AddReduceButtons(BUTTON_WIDTH, HEIGHT, ref organ.Y) || changed;
					Space(GAP_WIDTH_ALT);

					// Z
					GUI.enabled = organ.EnableZ;
					GUI.color = GUI.enabled ? oldC : tintColor;
					changed = IntField(LABEL_WIDTH_ALT, HEIGHT, "", ref organ.Z, GUI.enabled) || changed;
					changed = AddReduceButtons(BUTTON_WIDTH, HEIGHT, ref organ.Z) || changed;

					GUI.enabled = oldE;
					GUI.color = oldC;
				});

				Space(GAP_HEIGHT);

				LayoutH(() => {
					// Skin Color
					GUI.Label(GUIRect(LABEL_WIDTH, HEIGHT), "Skin");
					var color = ColorField(0, HEIGHT, "", organ.SkinColor);
					if (color != organ.SkinColor) {
						organ.SkinColor = color;
						changed = true;
					}

					// Mirror
					if (organ2 != null) {
						GUIRect(24, HEIGHT);
						var newMirror = EditorGUI.ToggleLeft(GUIRect(LABEL_WIDTH + 8, HEIGHT), " Mirror", organ2.Mirrored);
						if (newMirror != organ2.Mirrored) {
							organ2.Mirrored = newMirror;
							changed = true;
						}
					}
				});

				if (changed && organ2 != null && organ2.Mirrored) {
					// Mirror to another
					var otherOrgan = organ == organ2.Left ? organ2.Right : organ2.Left;
					otherOrgan.CopyFrom(organ, true);
				}

			}, false, new GUIStyle(GUI.skin.box) {
				fixedWidth = LABEL_WIDTH + LABEL_WIDTH_ALT * 3f + BUTTON_WIDTH * 3f + GAP_WIDTH_ALT * 2f + 12,
			});

			return changed;
		}



		#endregion




		#region --- LGC ---



		private void SwitchGeneratorMode (GeneratorMode mode) {
			CurrentModelIndex = 0;
			VoxelFilePath = "";
			CurrentGeneratorMode = mode;
			Data = VoxelData.CreateNewData();
			// Node Open
			if (Data) {
				foreach (var gp in Data.Groups) {
					if (!NodeOpen.ContainsKey(gp.Key)) {
						NodeOpen.Add(gp.Key, false);
					}
				}
			}
			// Switch Model
			SwitchModel(CurrentModelIndex);
			Load_Generation();
			DataDirty = false;
		}



		private void Generate () {
			try {
				switch (CurrentGeneratorMode) {
					case GeneratorMode.Character:
						CGConfig.FixGenerationValues();
						Data = Core_CharacterGeneration.Generate(CGConfig, ProgressStep_Cha);
						break;
					case GeneratorMode.Map:
						MGConfig.FixGenerationValues();
						Data = Core_MapGeneration.Generate(MGConfig, ProgressStep_Map);
						break;
				}
				SwitchModel(CurrentModelIndex, true);
				SetDataDirty();
			} catch (System.Exception ex) {
				Debug.LogWarning("[Voxel Generator] " + ex.Message);
			}
		}



		private void ImportPreset_Dialog () {
			try {
				string path = EditorUtility.OpenFilePanel("Import Generation Preset", "Assets", "json");
				if (!string.IsNullOrEmpty(path)) {
					var json = Util.Read(path);
					if (!string.IsNullOrEmpty(json)) {
						switch (CurrentGeneratorMode) {
							case GeneratorMode.Character:
								CGConfig.LoadFromJson(json);
								break;
							case GeneratorMode.Map:
								MGConfig.LoadFromJson(json);
								break;
						}
					}
				}
			} catch (System.Exception ex) {
				Debug.LogError(ex.Message);
			}
		}



		private void ExportPreset_Dialog () {
			try {
				var json = "";
				switch (CurrentGeneratorMode) {
					case GeneratorMode.Character:
						json = CGConfig.ToJson();
						break;
					case GeneratorMode.Map:
						json = MGConfig.ToJson();
						break;
				}
				if (!string.IsNullOrEmpty(json)) {
					string path = EditorUtility.SaveFilePanelInProject("Export Generation Preset", "Generation Preset", "json", "");
					if (!string.IsNullOrEmpty(path)) {
						Util.Write(json, path);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
				}
			} catch (System.Exception ex) {
				Debug.LogError(ex.Message);
			}
		}



		private void Load_Generation () {
			try {
				string rootPath = Util.GetRootPath(this);
				if (!string.IsNullOrEmpty(rootPath)) {
					string path = Util.CombinePaths(rootPath, "Data", "Current " + CurrentGeneratorMode.ToString() + " Generation Preset.json");
					if (Util.FileExists(path)) {
						switch (CurrentGeneratorMode) {
							case GeneratorMode.Map:
								MGConfig.LoadFromJson(Util.Read(path));
								break;
							case GeneratorMode.Character:
								CGConfig.LoadFromJson(Util.Read(path));
								break;
						}
					}
				}
			} catch { };
			PhysicalBodyTableOpen.Load();
			PhysicalBodyControllerOpen.Load();
			AttachmentControllerOpen.Load();
		}



		private void Save_Generation () {
			try {
				string rootPath = Util.GetRootPath(this);
				if (!string.IsNullOrEmpty(rootPath)) {
					string json = "";
					switch (CurrentGeneratorMode) {
						case GeneratorMode.Map:
							json = MGConfig.ToJson();
							break;
						case GeneratorMode.Character:
							json = CGConfig.ToJson();
							break;
					}
					if (!string.IsNullOrEmpty(json)) {
						Util.Write(json, Util.CombinePaths(rootPath, "Data", "Current " + CurrentGeneratorMode.ToString() + " Generation Preset.json"));
					}
				}
			} catch { };
		}



		private void Save_Generation_UI () {
			PhysicalBodyTableOpen.TrySave();
			PhysicalBodyControllerOpen.TrySave();
			AttachmentControllerOpen.TrySave();
		}



		#endregion



		#region --- UTL ---



		private void ProgressStep_Map (float progress01, int step) {
			ProgressStep(progress01, step, GeneratorMode.Map);
		}



		private void ProgressStep_Cha (float progress01, int step) {
			ProgressStep(progress01, step, GeneratorMode.Character);
		}



		private void ProgressStep (float progress01, int step, GeneratorMode mod) {
			string[] labels = mod == GeneratorMode.Map ? MAP_STEP_LABELS : CHARACTER_STEP_LABELS;
			if (step >= labels.Length) {
				Util.ClearProgressBar();
				return;
			}
			step = Mathf.Clamp(step, 0, labels.Length - 1);
			Util.ProgressBar(
				"Working...",
				string.Format(
					labels[step],
					(step + 1).ToString(),
					labels.Length.ToString(),
					(progress01 * 100).ToString("00")
				),
				progress01 / labels.Length + step / (labels.Length - 1f)
			);
		}




		#endregion

	}
}