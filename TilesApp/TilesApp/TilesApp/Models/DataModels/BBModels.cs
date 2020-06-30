﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models.DataModels
{

    public class Web_ProtoFamily
    {
        public int Id { get; set; }
        public string CosmoId { get; set; }
        public string ProjectId { get; set; }
        public string CategoryId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int VersionId { get; set; }
        public string VersionName { get; set; }
        public string Created_at { get; set; }
        public string Updated_at { get; set; }
        public string Deleted_at { get; set; }
    }

    public class Web_FamilyFields
    {
        public string ProtoFamilyId { get; set; }
        public int Phase { get; set; }
        public string FieldId { get; set; }
        public int UIIndex { get; set; }
    }

    public class Web_Field
    {
        public string CosmoId { get; set; }
        public string ProjectId { get; set; }
        public int FieldCategory { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public int Variant { get; set; }
        public string MongoSlug { get; set; }
        public string Slug { get; set; }
        public int PrimitiveType { get; set; }
        public int PrimitiveQuantity { get; set; }
        public string ValueRegEx { get; set; }
        public object Default { get; set; }
        public bool ValueIsUnique { get; set; }
        public bool ValueIsRequired { get; set; }
        public bool ValueIsForeignKey { get; set; }
        public string Created_at { get; set; }
        public string Updated_at { get; set; }
        public string Deleted_at { get; set; }
    }
}