using System.ComponentModel;

namespace EDO_FOMS.Domain.Enums
{
    public enum UploadType : byte
    {
        [Description(@"users")]
        ProfilePicture,

        [Description(@"docs")]
        Document,

        [Description(@"archive")]
        Archive,

        [Description(@"manual")]
        Manual
    }
}
