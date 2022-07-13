using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace EDO_FOMS.Application.Models.Dir
{
    [XmlRoot("packet")]
    public class XmlF001
    {
        [XmlElement("TFOMS")]
        public List<TFOMS> TFOMSs { get; set; }
    }

    public partial class TFOMS
    {
        [XmlElement("tf_kod")]
        public string TfKod { get; set; }

        [XmlElement("tf_okato")]
        public string TfOkato { get; set; }

        [XmlElement("tf_ogrn")]
        public string TfOgrn { get; set; }

        [XmlElement("name_tfp")]
        public string NameTfp { get; set; }

        [XmlElement("name_tfk")]
        public string NameTfk { get; set; }

        [XmlElement("index")]
        public string Index { get; set; }

        [XmlElement("address")]
        public string Address { get; set; }

        [XmlElement("fam_dir")]
        public string FamDir { get; set; }

        [XmlElement("im_dir")]
        public string ImDir { get; set; }

        [XmlElement("ot_dir")]
        public string OtDir { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }

        [XmlElement("fax")]
        public string Fax { get; set; }

        [XmlElement("hot_line")]
        public string HotLine { get; set; }

        [XmlElement("e_mail")]
        public string EMail { get; set; }

        [XmlElement("kf_tf")]
        public int KfTf { get; set; }

        [XmlElement("www")]
        public string Www { get; set; }

        [XmlElement("MTR")]
        public Mtr Mtr { get; set; }

        public DateTime? _DEdit { get; set; }

        [XmlElement("d_edit")]
        public string DEdit
        {
            get { return _DEdit?.ToString("dd.MM.yyyy"); }
            set { _DEdit = string.IsNullOrWhiteSpace(value) ? null : DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture); }
        }

        public DateTime? _DEnd { get; set; }

        [XmlElement("d_end")]
        public string DEnd {
            get { return _DEnd?.ToString("dd.MM.yyyy"); }
            set { _DEnd = string.IsNullOrWhiteSpace(value) ? null : DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture); }
        }
    }

    public struct Mtr
    {
        [XmlElement("bic")]
        public string Bic { get; set; }

        [XmlElement("inn")]
        public string Inn { get; set; }

        [XmlElement("kpp")]
        public string Kpp { get; set; }

        [XmlElement("kbk")]
        public string Kbk { get; set; }

        [XmlElement("oktmo")]
        public string Oktmo { get; set; }

        [XmlElement("MTR_POL")]
        public MtrPol MtrPol { get; set; }

        [XmlElement("MTR_PL")]
        public MtrPl MtrPl { get; set; }
    }

    public struct MtrPol
    {
        [XmlElement("L_NAIM")]
        public string LNaim { get; set; }

        [XmlElement("L_B")]
        public string LB { get; set; }

        [XmlElement("L_RS")]
        public string LRs { get; set; }
    }

    public struct MtrPl
    {
        [XmlElement("T_NAIM")]
        public string TNaim { get; set; }

        [XmlElement("T_B")]
        public string TB { get; set; }

        [XmlElement("T_RS")]
        public string TRs { get; set; }
    }
}
