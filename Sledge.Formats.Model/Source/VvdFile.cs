using Sledge.Formats.FileSystem;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sledge.Formats.Model.Source
{
	public class VvdFile
	{
		public VvdHeader Header { get; set; }
		public StudioVertex[] Vertices { get; set; }
		public Vector4[] TangentData { get; set; }
		public VvdFile(Stream stream)
		{
			var headerBuf = new byte[Marshal.SizeOf<VvdHeader>()];
			var vertexSize = Marshal.SizeOf<StudioVertex>();
			stream.Read(headerBuf, 0, headerBuf.Length);
			var handle = GCHandle.Alloc(headerBuf, GCHandleType.Pinned);
			Header = Marshal.PtrToStructure<VvdHeader>(handle.AddrOfPinnedObject());
			handle.Free();

			var vertexCount = (Header.tangentDataStart - Header.vertexDataStart) / vertexSize;
			var vertexBuf = new byte[vertexCount * vertexSize];
			stream.Seek(Header.vertexDataStart, SeekOrigin.Begin);
			stream.Read(vertexBuf, 0, vertexBuf.Length);
			var vertexHandle = GCHandle.Alloc(vertexBuf, GCHandleType.Pinned);
			Vertices = new StudioVertex[vertexCount];
			for(int i = 0; i < vertexCount; i++)
			{
				var offset = i * vertexSize;
				Vertices[i] = Marshal.PtrToStructure<StudioVertex>(vertexHandle.AddrOfPinnedObject() + offset);
			}
			vertexHandle.Free();

			var tangentBuf = new byte[Marshal.SizeOf<Vector4>()];
			var tangentSize = Marshal.SizeOf<Vector4>();
			stream.Read(tangentBuf, 0, tangentBuf.Length);
			var tangHandle = GCHandle.Alloc(tangentBuf, GCHandleType.Pinned);
			TangentData = new Vector4[vertexCount];
			for (int i = 0; i < vertexCount; i++)
			{
				var offset = i * tangentSize;
				TangentData[i] = Marshal.PtrToStructure<Vector4>(tangHandle.AddrOfPinnedObject() + offset);
			}
			tangHandle.Free();
		}

		public static VvdFile FromFile(string path)
		{
			var dir = Path.GetDirectoryName(path);
			var fname = Path.GetFileName(path);

			var resolver = new DiskFileResolver(dir);
			return FromFile(resolver, fname);
		}
		public static VvdFile FromFile(IFileResolver resolver, string path)
		{
			var basedir = (Path.GetDirectoryName(path) ?? "").Replace('\\', '/');
			if (basedir.Length > 0 && !basedir.EndsWith("/")) basedir += "/";
			var basepath = basedir + Path.GetFileNameWithoutExtension(path);
			var ext = Path.GetExtension(path);

			try
			{
				var stream = resolver.OpenFile(path);

				return new VvdFile(stream);
			}
			finally
			{
			}
		}

	}
}
