﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace EDO_FOMS.Application.Models.Dir
{
    [XmlRoot("packet")]
    public class XmlF002
    {
        [XmlElement("insCompany")]
        public List<SMO> SMOs { get; set; }
    }

    public partial class SMO
    {
        [XmlElement("tf_okato")]
        public string TfOkato { get; set; }

        [XmlElement("smocod")]
        public string SmoCod { get; set; }

        [XmlElement("inn")]
        public string Inn { get; set; }

        [XmlElement("KPP")]
        public string Kpp { get; set; }

        [XmlElement("Ogrn")]
        public string Ogrn { get; set; }

        [XmlElement("nam_smop")]
        public string NamSmop { get; set; }

        [XmlElement("nam_smok")]
        public string NamSmok { get; set; }

        [XmlElement("phone")]
        public string Phone { get; set; }

        [XmlElement("fax")]
        public string Fax { get; set; }

        [XmlElement("hot_line")]
        public string HotLine { get; set; }

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

        [XmlElement("pstAddress")]
        public PstAddress PstAddress { get; set; }

        public DateTime? _DEdit { get; set; }

        [XmlElement("d_edit")]
        public string DEdit
        {
            get { return _DEdit?.ToString("dd.MM.yyyy"); }
            set { _DEdit = string.IsNullOrWhiteSpace(value) ? null : DateTime.ParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture); }
        }
    }

    public struct JurAddress
    {
        [XmlElement("index_j")]
        public string IndexJ { get; set; }

        [XmlElement("addr_j")]
        public string AddrJ { get; set; }
    }

    public struct PstAddress
    {
        [XmlElement("index_f")]
        public string IndexF { get; set; }

        [XmlElement("addr_f")]
        public string AddrF { get; set; }
    }
}
