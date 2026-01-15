using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Starsbane.AI.Amazon
{
    [JsonConverter(typeof(StringEnumConverter))]
    internal enum Quality {
        [EnumMember(Value = "standard")]
        Standard,

        [EnumMember(Value = "premium")]
        Premium
    }

    [JsonConverter(typeof(StringEnumConverter))]
    internal enum GenerateImageStyle
    {
        [EnumMember(Value = "3D_ANIMATED_FAMILY_FILM")]
        ThreeDimensionAnimatedFamilyFilm,

        [EnumMember(Value = "DESIGN_SKETCH")]
        DesignSketch,

        [EnumMember(Value = "FLAT_VECTOR_ILLUSTRATION")]
        FlatVectorIllustration,

        [EnumMember(Value = "GRAPHIC_NOVEL_ILLUSTRATION")]
        GraphicNovelIllustration,

        [EnumMember(Value = "MAXIMALISM")]
        Maximalism,

        [EnumMember(Value = "MIDCENTURY_RETRO")]
        MidcenturyRetro,

        [EnumMember(Value = "PHOTOREALISM")]
        Photorealism,

        [EnumMember(Value = "SOFT_DIGITAL_PAINTING")]
        SoftDigitalPainting
    }
}
