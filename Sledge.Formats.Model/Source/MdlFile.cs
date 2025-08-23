using Sledge.Formats.FileSystem;
using System.IO;
using System.Runtime.InteropServices;

namespace Sledge.Formats.Model.Source
{
	public class MdlFile
	{
		public Studiohdr Header { get; set; }
		public Bone[] Bones { get; set; }
		// TODO: BoneControllers
		public HitboxSet[] HitboxSets { get; set; }
		public AnimDescription[] AnimDescriptions { get; set; }



		public string[] Materials { get; set; }
		public string MaterialDirectory { get; set; }

		public VtxFile VtxFile { get; set; }
		public VvdFile VvdFile { get; set; }


		public MdlFile(Stream stream)
		{
			using (var br = new BinaryReader(stream))
			{
				var buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
				Header = Marshal.PtrToStructure<Studiohdr>(handle.AddrOfPinnedObject());

				Bones = new Bone[Header.bone_count];
				for (int i = 0; i < Header.bone_count; i++)
				{
					Bones[i] = new Bone();
					var offset = Header.bone_offset + i * Marshal.SizeOf<StudioHdrBone>();

					Bones[i].ReadObjects(handle, br, offset);
				}
				HitboxSets = new HitboxSet[Header.hitbox_count];
				for (int i = 0; i < Header.hitbox_count; i++)
				{
					HitboxSets[i] = new HitboxSet();
					var offset = Header.hitbox_offset + i * Marshal.SizeOf<StudioHdrHitboxSet>();
					HitboxSets[i].ReadObjects(handle, br, offset);
				}
				AnimDescriptions = new AnimDescription[Header.localanim_count];
				for (int i = 0; i < Header.localanim_count; i++)
				{
					AnimDescriptions[i] = new AnimDescription();
					var offset = Header.localanim_offset + i * Marshal.SizeOf<StudioHdrAnimDesc>();
					AnimDescriptions[i].ReadObjects(handle, br, offset);
				}


				var materialCount = Header.texture_count;
				Materials = new string[materialCount];
				stream.Seek(Header.texture_offset, SeekOrigin.Begin);


				int texturesOffset = br.ReadInt32();
				stream.Seek(texturesOffset + Header.texture_offset, SeekOrigin.Begin);
				for (int i = 0; i < materialCount; i++)
				{
					Materials[i] = br.ReadNullTerminatedString();
				}
				stream.Seek(Header.texturedir_offset, SeekOrigin.Begin);
				var dirOffset = br.ReadInt32();
				stream.Seek(dirOffset, SeekOrigin.Begin);
				MaterialDirectory = br.ReadNullTerminatedString();
				handle.Free();
			}

		}

		public static MdlFile FromFile(string path)
		{
			var dir = Path.GetDirectoryName(path);
			var fname = Path.GetFileName(path);

			var resolver = new DiskFileResolver(dir);
			var file = FromFile(resolver, fname);
			// TODO: Prioritize dx90
			var vtxPath = Path.ChangeExtension(path, ".dx90.vtx");
			if (resolver.FileExists(vtxPath))
			{
				file.VtxFile = VtxFile.FromFile(resolver, vtxPath);
			}
			var vvdPath = Path.ChangeExtension(path, ".vvd");
			if (resolver.FileExists(vvdPath))
			{
				file.VvdFile = VvdFile.FromFile(resolver, vvdPath);
			}

			return file;
		}
		public static MdlFile FromFile(IFileResolver resolver, string path)
		{
			var basedir = (Path.GetDirectoryName(path) ?? "").Replace('\\', '/');
			if (basedir.Length > 0 && !basedir.EndsWith("/")) basedir += "/";
			var basepath = basedir + Path.GetFileNameWithoutExtension(path);
			var ext = Path.GetExtension(path);

			try
			{
				var stream = resolver.OpenFile(path);

				return new MdlFile(stream);
			}
			finally
			{
			}
		}
	}
}
