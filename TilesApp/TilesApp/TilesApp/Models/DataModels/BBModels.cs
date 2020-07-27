using MongoDB.Bson.Serialization.Attributes;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TilesApp.Models.DataModels
{
    public class Web_Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
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
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ProtoFamilyId { get; set; }
        public string FieldId { get; set; }
        public string EntityId { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public string Phase { get; set; }
        public string Created_at { get; set; }
        public string Updated_at { get; set; }
        public string Deleted_at { get; set; }
        public string PrimitiveType { get; set; }
        public int PrimitiveQuantity { get; set; }
        public string FieldCategory { get; set; }
        public int EntityType { get; set; }
        public int UIindex { get; set; }
        public string ValueRegEx { get; set; }
        public bool ValueIsUnique { get; set; }
        public bool ValueIsRequired { get; set; }
        public bool Index { get; set; }
        public bool Show { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public object Default { get; set; }

    }

    public class Form_Info
    {
        [BsonIgnoreIfNull]
        public object pha
        {
            get
            {
                return 0;
            }
        }

        [BsonIgnoreIfNull]
        public Dictionary<string, object> FieldsData { get; set; }

        [BsonIgnoreIfNull]
        public string UserName
        {
            get
            {
                return App.User.DisplayName;
            }
        }

        public Form_Info(Dictionary<string, object> listFields)
        {
            FieldsData = listFields;
        }
    }
}
