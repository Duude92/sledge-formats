using System.Runtime.InteropServices;

namespace Sledge.Formats.Model.Source
{
	// this structure is in <mod folder>/src/public/optimize.h
	public struct VtxHeader
	{
		// file version as defined by OPTIMIZED_MODEL_FILE_VERSION (currently 7)
		public int version;

		// hardware params that affect how the model is to be optimized.
		public int vertCacheSize;
		public ushort maxBonesPerStrip;
		public ushort maxBonesPerTri;
		public int maxBonesPerVert;

		// must match checkSum in the .mdl
		public int checkSum;

		public int numLODs; // Also specified in ModelHeader_t's and should match

		// Offset to materialReplacementList Array. one of these for each LOD, 8 in total
		public int materialReplacementListOffset;

		//Defines the size and location of the body part array
		public int numBodyParts;
		public int bodyPartOffset;
	};
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct BodyPartHeader
	{
		//Model array
		public int numModels;
		public int modelOffset;
		public ModelHeader ModelHeader;

	};
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ModelHeader
	{
		//LOD mesh array
		public int numLODs;   //This is also specified in FileHeader_t
		public int lodOffset;
		public ModelLODHeader ModelLOD;
	};
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ModelLODHeader
	{
		//Mesh array
		public int numMeshes;
		public int meshOffset;
		public float switchPoint;
		public MeshHeader MeshHeader;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct MeshHeader
	{
		public int numStripGroups;
		public int stripGroupHeaderOffset;
		public byte flags;
		public StripGroupHeader StripGroupHeader;
	};
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct StripGroupHeader
	{
		// These are the arrays of all verts and indices for this mesh.  strips index into this.
		public int numVerts;
		public int vertOffset;

		public int numIndices;
		public int indexOffset;

		public int numStrips;
		public int stripOffset;


		public byte flags;
		public StripHeader StripHeader;

		// The following fields are only present if MDL version is >=49
		// Points to an array of unsigned shorts (16 bits each)
		//public int numTopologyIndices;
		//public int topologyOffset;
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct StripHeader
	{
		public int numIndices;
		public int indexOffset;
		public int numVerts;
		public int vertOffset;
		public short numBones;
		public byte flags;
		public int numBoneStateChanges;
		public int boneStateChangeOffset;

		//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
		//public Vertex[] verts;
		//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
		//public ushort[] indices;

		// MDL Version 49 and up only
		//public int numTopologyIndices;
		//public int topologyOffset;
	};
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vertex
	{
		// these index into the mesh's vert[origMeshVertID]'s bones
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[] boneWeightIndex;
		public byte numBones;
		public ushort origMeshVertID;

		// for sw skinned verts, these are indices into the global list of bones
		// for hw skinned verts, these are hardware bone indices
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
		public byte[] boneID;
	};
	public struct MaterialReplacementListHeader
	{
		public int numReplacements;
		public int replacementOffset;
	};
}
