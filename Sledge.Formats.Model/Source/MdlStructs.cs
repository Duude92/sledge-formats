using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Formats.Model.Source
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Studiohdr
	{
		public int id;             // Model format ID, such as "IDST" (0x49 0x44 0x53 0x54)
		public int version;        // Format version number, such as 48 (0x30,0x00,0x00,0x00)
		public int checksum;       // This has to be the same in the phy and vtx files to load!
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
		public char[] name;       // The internal name of the model, padding with null bytes.
								  // Typically "my_model.mdl" will have an internal name of "my_model"
		public int dataLength;     // Data size of MDL file in bytes.

		// A vector is 12 bytes, three 4-byte float-values in a row.
		public Vector3 eyeposition;    // Position of player viewpoint relative to model origin
		public Vector3 illumposition;  // Position (relative to model origin) used to calculate ambient light contribution and cubemap reflections for the entire model.
		public Vector3 hull_min;       // Corner of model hull box with the least X/Y/Z values
		public Vector3 hull_max;       // Opposite corner of model hull box
		public Vector3 view_bbmin;     // Same, but for bounding box,
		public Vector3 view_bbmax;     // which is used for view culling

		public int flags;          // Binary flags in little-endian order. 
								   // ex (0x010000C0) means flags for position 0, 30, and 31 are set. 
								   // Set model flags section for more information

		/*
		 * After this point, the header contains many references to offsets
		 * within the MDL file and the number of items at those offsets.
		 *
		 * Offsets are from the very beginning of the file.
		 * 
		 * Note that indexes/counts are not always paired and ordered consistently.
		 */

		// mstudiobone_t
		public int bone_count;    // Number of data sections (of type mstudiobone_t)
		public int bone_offset;   // Offset of first data section

		// mstudiobonecontroller_t
		public int bonecontroller_count;
		public int bonecontroller_offset;

		// mstudiohitboxset_t
		public int hitbox_count;
		public int hitbox_offset;

		// mstudioanimdesc_t
		public int localanim_count;
		public int localanim_offset;

		// mstudioseqdesc_t
		public int localseq_count;
		public int localseq_offset;

		public int activitylistversion; // ??
		public int eventsindexed;       // ??

		// VMT texture filenames
		// mstudiotexture_t
		public int texture_count;
		public int texture_offset;

		// This offset points to a series of ints.
		// Each int value, in turn, is an offset relative to the start of this header/the-file,
		// At which there is a null-terminated string.
		public int texturedir_count;
		public int texturedir_offset;

		// Each skin-family assigns a texture-id to a skin location
		public int skinreference_count;
		public int skinrfamily_count;
		public int skinreference_index;

		// mstudiobodyparts_t
		public int bodypart_count;
		public int bodypart_offset;

		// Local attachment points        
		// mstudioattachment_t
		public int attachment_count;
		public int attachment_offset;

		// Node values appear to be single bytes, while their names are null-terminated strings.
		public int localnode_count;
		public int localnode_index;
		public int localnode_name_index;

		// mstudioflexdesc_t
		public int flexdesc_count;
		public int flexdesc_index;

		// mstudioflexcontroller_t
		public int flexcontroller_count;
		public int flexcontroller_index;

		// mstudioflexrule_t
		public int flexrules_count;
		public int flexrules_index;

		// IK probably referse to inverse kinematics
		// mstudioikchain_t
		public int ikchain_count;
		public int ikchain_index;

		// Information about any "mouth" on the model for speech animation
		// More than one sounds pretty creepy.
		// mstudiomouth_t
		public int mouths_count;
		public int mouths_index;

		// mstudioposeparamdesc_t
		public int localposeparam_count;
		public int localposeparam_index;

		/*
		 * For anyone trying to follow along, as of this writing,
		 * the next "surfaceprop_index" value is at position 0x0134 (308)
		 * from the start of the file.
		 */

		// Surface property value (single null-terminated string)
		public int surfaceprop_index;

		// Unusual: In this one index comes first, then count.
		// Key-value data is a series of strings. If you can't find
		// what you're interested in, check the associated PHY file as well.
		public int keyvalue_index;
		public int keyvalue_count;

		// More inverse-kinematics
		// mstudioiklock_t
		public int iklock_count;
		public int iklock_index;


		public float mass;      // Mass of object (4-bytes) in kilograms

		public int contents;    // contents flag, as defined in bspflags.h
								// not all content types are valid; see 
								// documentation on $contents QC command

		// Other models can be referenced for re-used sequences and animations
		// (See also: The $includemodel QC option.)
		// mstudiomodelgroup_t
		public int includemodel_count;
		public int includemodel_index;

		public int virtualModel;    // Placeholder for mutable-void*
									// Note that the SDK only compiles as 32-bit, so an int and a pointer are the same size (4 bytes)

		// mstudioanimblock_t
		public int animblocks_name_index;
		public int animblocks_count;
		public int animblocks_index;

		public int animblockModel; // Placeholder for mutable-void*

		// Points to a series of bytes?
		public int bonetablename_index;

		public int vertex_base;    // Placeholder for void*
		public int offset_base;    // Placeholder for void*

		// Used with $constantdirectionallight from the QC 
		// Model should have flag #13 set if enabled
		public byte directionaldotproduct;

		public byte rootLod;    // Preferred rather than clamped

		// 0 means any allowed, N means Lod 0 -> (N-1)
		public byte numAllowedRootLods;

		public byte unused0; // ??
		public int unused1; // ??

		// mstudioflexcontrollerui_t
		public int flexcontrollerui_count;
		public int flexcontrollerui_index;

		public float vertAnimFixedPointScale; // ??
		public int unused2;

		/**
		 * Offset for additional header information.
		 * May be zero if not present, or also 408 if it immediately 
		 * follows this studiohdr_t
		 */
		// studiohdr2_t
		public int studiohdr2index;

		public int unused3; // ??

		/**
		 * As of this writing, the header is 408 bytes long in total
		 */
	};
	public class Bone
	{
		public StudioHdrBone Data { get; set; }
		public string BoneName { get; set; }
		internal void ReadObjects(GCHandle handle, BinaryReader br, int offset)
		{
			Data = Marshal.PtrToStructure<StudioHdrBone>(handle.AddrOfPinnedObject() + offset);
			br.BaseStream.Seek(offset + Data.bone_name_offset, SeekOrigin.Begin);
			BoneName = br.ReadNullTerminatedString();
		}
	}
	public struct StudioHdrBone
	{
		public uint bone_name_offset;
		public int parent;     // parent bone
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public int[] bonecontroller;  // bone controller index, -1 == none
									  // default values
		public Vector3 pos;
		public Vector4 quat;
		public Vector3 rot;
		// compression scale
		public Vector3 posscale;
		public Vector3 rotscale;
		public Matrix3x4 poseToBone;
		public Vector4 qAlignment;
		public int flags;
		public int proctype;
		public int procindex;      // procedural rule
		public int physicsbone;    // index into physically simulated bone

		public int surfacepropidx; // index into string tablefor property name

		public int contents;       // See BSPFlags.h for the contents flags
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public int[] unused;      // remove as appropriate
	}
	public class HitboxSet
	{
		public StudioHdrHitboxSet Header { get; set; }
		public string Name { get; set; }
		public Hitbox[] Hitboxes { get; set; }

		internal void ReadObjects(GCHandle handle, BinaryReader br, int offset)
		{
			Header = Marshal.PtrToStructure<StudioHdrHitboxSet>(handle.AddrOfPinnedObject() + offset);
			br.BaseStream.Seek(offset + Header.sznameindex, SeekOrigin.Begin);
			Name = br.ReadNullTerminatedString();
			Hitboxes = new Hitbox[Header.numhitboxes];
			for (int i = 0; i < Header.numhitboxes; i++)
			{
				Hitboxes[i] = new Hitbox();
				var hitboxOffset = offset + Header.hitboxindex + i * Marshal.SizeOf<StudioHdrHitbox>();
				Hitboxes[i].ReadObjects(handle, br, hitboxOffset);
			}
		}
		public class Hitbox
		{
			public StudioHdrHitbox Data { get; set; }
			public string Name { get; set; }
			internal void ReadObjects(GCHandle handle, BinaryReader br, int offset)
			{
				Data = Marshal.PtrToStructure<StudioHdrHitbox>(handle.AddrOfPinnedObject() + offset);
				br.BaseStream.Seek(Data.szhitboxnameindex, SeekOrigin.Begin);
				Name = br.ReadNullTerminatedString();
			}
		}

	}

	public struct StudioHdrHitbox
	{
		public int bone;
		public int group;              // intersection group
		public Vector3 bbmin;              // bounding box
		public Vector3 bbmax;
		public int szhitboxnameindex;  // offset to the name of the hitbox.
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public int[] unused;

	};
	public struct StudioHdrHitboxSet
	{
		public int sznameindex;
		public int numhitboxes;
		public int hitboxindex;
	};

	public class AnimDescription
	{
		public StudioHdrAnimDesc Data { get; set; }
		public string Name { get; set; }
		internal void ReadObjects(GCHandle handle, BinaryReader br, int offset)
		{
			Data = Marshal.PtrToStructure<StudioHdrAnimDesc>(handle.AddrOfPinnedObject() + offset);
			br.BaseStream.Seek(offset + Data.sznameindex, SeekOrigin.Begin);
			Name = br.ReadNullTerminatedString();
		}
	}
	public struct StudioHdrAnimDesc
	{
		public int baseptr;
		public int sznameindex;
		public float fps;      // frames per second	
		public int flags;      // looping/non-looping flags
		public int numframes;
		
		// piecewise movement
		public int nummovements;
		public int movementindex;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		public int[] unused1;         // remove as appropriate (and zero if loading older versions)	
		public int animblock;
		public int animindex;   // non-zero when anim data isn't in sections
		public int numikrules;
		public int ikruleindex;    // non-zero when IK data is stored in the mdl
		public int animblockikruleindex; // non-zero when IK data is stored in animblock file
		public int numlocalhierarchy;
		public int localhierarchyindex;
		public int sectionindex;
		public int sectionframes; // number of frames used in each fast lookup section, zero if not used
		public short zeroframespan;    // frames per span
		public short zeroframecount; // number of spans
		public int zeroframeindex;
		public float zeroframestalltime;       // saved during read stalls
	};

	public struct Matrix3x4
	{
		public float m11;
		public float m12;
		public float m13;
		public float m21;
		public float m22;
		public float m23;
		public float m31;
		public float m32;
		public float m33;
		public float m41;
		public float m42;
		public float m43;
	}
}
