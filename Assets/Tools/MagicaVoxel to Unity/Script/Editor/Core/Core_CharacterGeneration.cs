namespace VoxeltoUnity {
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public static class Core_CharacterGeneration {




		#region --- SUB ---




		[System.Serializable]
		public class OrganData {

			public int X;
			public int Y;
			public int Z;
			public bool EnableX = true;
			public bool EnableY = true;
			public bool EnableZ = true;
			public int SizeX;
			public int SizeY;
			public int SizeZ;
			public Color SkinColor = new Color(231f / 255f, 181f / 255f, 145f / 255f);

			public OrganData (int sizeX, int sizeY, int sizeZ, int x, int y, int z, bool eX, bool eY, bool eZ) {
				X = x;
				Y = y;
				Z = z;
				SizeX = sizeX;
				SizeY = sizeY;
				SizeZ = sizeZ;
				EnableX = eX;
				EnableY = eY;
				EnableZ = eZ;
			}

			public void CopyFrom (OrganData other, bool mirror = false) {
				X = mirror ? -other.X : other.X;
				Y = other.Y;
				Z = other.Z;
				SizeX = other.SizeX;
				SizeY = other.SizeY;
				SizeZ = other.SizeZ;
			}

			public static implicit operator bool (OrganData organ) {
				return organ != null;
			}
		}



		[System.Serializable]
		public class OrganData2 {

			public OrganData Left;
			public OrganData Right;
			public bool Mirrored = true;

			public OrganData2 (OrganData l, OrganData r) {
				Left = l;
				Right = r;
			}
			public OrganData2 (int w, int h, int t, int x, int y, int z, bool ex, bool ey, bool ez) {
				Left = new OrganData(w, h, t, x, y, z, ez, ey, ez);
				Right = new OrganData(w, h, t, x, y, z, ex, ey, ez);
			}
		}



		public class OrganTransformInfo {
			public OrganData Parent;
			public int PositionX;
			public int PositionY;
			public int PositionZ;
			public static implicit operator bool (OrganTransformInfo info) {
				return info != null;
			}
		}



		[System.Serializable]
		public class Preset {


			// VAR
			public OrganData Head = new OrganData(7, 7, 7, 0, 0, 0, true, false, true);
			public OrganData Neck = new OrganData(3, 1, 3, 0, 0, 0, true, false, true);
			public OrganData Body = new OrganData(5, 4, 3, 0, 0, 0, true, false, true);
			public OrganData Hip = new OrganData(5, 1, 3, 0, 0, 0, false, false, false);
			public OrganData2 LegU = new OrganData2(1, 3, 1, 0, 0, 0, true, false, true);
			public OrganData2 LegD = new OrganData2(1, 2, 1, 0, 0, 0, true, false, true);
			public OrganData2 Foot = new OrganData2(1, 1, 1, 0, 0, 0, true, false, true);
			public OrganData2 ArmU = new OrganData2(1, 3, 1, 0, 0, 0, false, true, true);
			public OrganData2 ArmD = new OrganData2(1, 2, 1, 0, 0, 0, false, true, true);
			public OrganData2 Hand = new OrganData2(1, 1, 1, 0, 0, 0, false, true, true);



			// API
			public void LoadFromJson (string json) {
				try {
					if (string.IsNullOrEmpty(json)) { return; }
					JsonUtility.FromJsonOverwrite(json, this);
				} catch { }
			}



			public string ToJson () {
				return JsonUtility.ToJson(this, true);
			}



			public void FixGenerationValues () {
				FixOrganValue(Head);
				FixOrganValue(Neck);
				FixOrganValue(Body);
				FixOrganValue(Hip);
				FixOrganValue(LegU);
				FixOrganValue(LegD);
				FixOrganValue(Foot);
				FixOrganValue(ArmU);
				FixOrganValue(ArmD);
				FixOrganValue(Hand);

				Head.Y = 0;
				Neck.Y = 0;
				Body.Y = 0;
				Hip.Y = 0;

				LegU.Left.Y = 0;
				LegU.Right.Y = 0;
				LegD.Left.Y = 0;
				LegD.Right.Y = 0;
				Foot.Left.Y = 0;
				Foot.Right.Y = 0;

				ArmU.Left.X = 0;
				ArmU.Right.X = 0;
				ArmD.Left.X = 0;
				ArmD.Right.X = 0;
				Hand.Left.X = 0;
				Hand.Right.X = 0;

			}



			public void ForAllOrgans (System.Action<OrganData> action) {
				action(Head);
				action(Neck);
				action(Body);
				action(Hip);
				action(LegU.Left);
				action(LegD.Left);
				action(Foot.Left);
				action(ArmU.Left);
				action(ArmD.Left);
				action(Hand.Left);
				action(LegU.Right);
				action(LegD.Right);
				action(Foot.Right);
				action(ArmU.Right);
				action(ArmD.Right);
				action(Hand.Right);
			}



			public OrganTransformInfo GetTransformInfo (OrganData organ) {
				OrganTransformInfo info = new OrganTransformInfo();
				if (organ == Head) {
					info.Parent = Neck;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == Neck) {
					info.Parent = Body;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == Body) {
					info.Parent = Hip;
					//var parentInfo = GetTransformInfo(info.Parent);
					//info.PositionX = parentInfo.PositionX + Body.X;
					//info.PositionY = parentInfo.PositionY + Body.Y;
					//info.PositionZ = parentInfo.PositionZ + Body.Z;

				} else if (organ == Hip) {
					info.Parent = null;
					info.PositionX = Hip.X;
					info.PositionY = Hip.Y;
					info.PositionZ = Hip.Z;

				} else if (organ == ArmU.Left) {
					info.Parent = Body;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == ArmU.Right) {
					info.Parent = Body;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == ArmD.Left) {
					info.Parent = ArmU.Left;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == ArmD.Right) {
					info.Parent = ArmU.Right;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == Hand.Left) {
					info.Parent = ArmD.Left;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == Hand.Right) {
					info.Parent = ArmD.Right;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == LegU.Left) {
					info.Parent = Hip;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == LegU.Right) {
					info.Parent = Hip;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == LegD.Left) {
					info.Parent = LegU.Left;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == LegD.Right) {
					info.Parent = LegU.Right;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == Foot.Left) {
					info.Parent = LegD.Left;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else if (organ == Foot.Right) {
					info.Parent = LegD.Right;
					//var parentInfo = GetTransformInfo(info.Parent);


				} else {
					info = null;
				}
				return info;
			}



			// LGC
			private void FixOrganValue (OrganData2 organ) {
				FixOrganValue(organ.Left);
				FixOrganValue(organ.Right);
			}



			private void FixOrganValue (OrganData organ) {
				organ.SizeX = Mathf.Max(organ.SizeX, 0);
				organ.SizeY = Mathf.Max(organ.SizeY, 0);
				organ.SizeZ = Mathf.Max(organ.SizeZ, 0);
			}





		}



		#endregion




		public static VoxelData Generate (Preset preset, System.Action<float, int> onProgress = null) {
			try {
				var data = VoxelData.CreateNewData();
				if (preset == null) { return data; }


				// Physical Body
				if (onProgress != null) {
					onProgress(0f, 0);
				}
				//int minX = int.MaxValue;
				//int minY = int.MaxValue;
				//int minZ = int.MaxValue;
				//int maxX = int.MinValue;
				//int maxY = int.MinValue;
				//int maxZ = int.MinValue;
				preset.ForAllOrgans((organ) => {




				});





				// Attachment
				if (onProgress != null) {
					onProgress(0f, 1);
				}





				if (onProgress != null) {
					onProgress(1f, int.MaxValue);
				}
				return data;
			} catch (System.Exception ex) {
				if (onProgress != null) {
					onProgress(1f, int.MaxValue);
				}
				throw ex;
			}
		}



	}
}