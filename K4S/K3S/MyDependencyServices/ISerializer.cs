using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDependencyServices
{
    public interface ISerializer
    {
        T Decompress<T>(byte[] compressedData) where T:class;

        byte[] Compress<T>(T data) where T : class;

        byte[] ToBinary<T>(T data) where T : class;

        T FromBinary<T>(byte[] binary) where T : class;
    }
}
