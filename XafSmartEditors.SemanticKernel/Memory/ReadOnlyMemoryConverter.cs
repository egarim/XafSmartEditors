using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
namespace XafSmartEditors.SemanticKernel.Memory
{
    [ExcludeFromCodeCoverage]
    public sealed class ReadOnlyMemoryConverter : JsonConverter<ReadOnlyMemory<float>>
    {
        //
        // Summary:
        //     An instance of a converter for float[] that all operations delegate to.
        private static readonly JsonConverter<float[]> s_arrayConverter = (JsonConverter<float[]>)new JsonSerializerOptions().GetConverter(typeof(float[]));

        public override ReadOnlyMemory<float> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return s_arrayConverter.Read(ref reader, typeof(float[]), options).AsMemory();
        }

        public override void Write(Utf8JsonWriter writer, ReadOnlyMemory<float> value, JsonSerializerOptions options)
        {
            s_arrayConverter.Write(writer, MemoryMarshal.TryGetArray(value, out var segment) && segment.Count == value.Length ? segment.Array : value.ToArray(), options);
        }
    }
    // <summary>Caches common configurations of <see cref="JsonSerializerOptions"/>.</summary>\
}
