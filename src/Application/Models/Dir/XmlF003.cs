using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace EDO_FOMS.Application.Models.Dir
{
    [XmlRoot("packet")]
    public class XmlF003
    {
        [XmlElement("medCompany")]
        public List<MO> MOs { get; set; }
    }

    public partial class MO
    {
        [XmlElement("tf_okato")]
        public string TfOkato { get; set; }

        [XmlElement("mcod")]
        public string MCod { get; set; }

        [XmlElement("inn")]
        public string Inn { get; set; }

        [XmlElement("KPP")]
        public string Kpp { get; set; }

        [XmlElement("Ogrn")]
        public string Ogrn { get; set; }

        [XmlElement("nam_mop")]
        public string NamMop { get; set; }

        [XmlElement("nam_mok")]
        public string NamMok { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }

        [XmlElement("fax")]
        public string Fax { get; set; }

        [XmlElement("e_mail")]
        public string EMail { get; set; }

        [XmlElement("www")]
        public string Www { get; set; }

        [XmlElement("fam_ruk")]
        public string FamRuk { get; set; }

        [XmlElement("im_ruk")]
        public string ImRuk { get; set; }

        [XmlElement("ot_ruk")]
        public string OtRuk { get; set; }

        [XmlElement("jurAddress")]
        public JurAddress JurAddress { get; set; }

        public DateTime? _DEdit { get; set; }

        [XmlElement("d_edit")]
        public string DEdit
        {
            get { return _DEdit?.ToString("dd.MM.yyyy"); }
            set { _DEdit = string.IsNullOrWhiteSpace(value) ? null : DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture); }
        }
    }
}
