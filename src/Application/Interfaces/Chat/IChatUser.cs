using System.ComponentModel.DataAnnotations.Schema;

namespace EDO_FOMS.Application.Interfaces.Chat
{
    public interface IChatUser
    {
        public string UserName { get; set; }

        [Column(TypeName = "text")]
        public string ProfilePictureDataUrl { get; set; }
    }
}