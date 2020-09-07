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
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
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
        // General attributes
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public string ProtoFamilyId { get; set; }
        public string FieldId { get; set; }
        public string ValueRegEx { get; set; }
        public object Default { get; set; }
        public int PrimitiveQuantity { get; set; }
        public string EntityId { get; set; }
        public string Phases { get; set; }
        public int UIindex { get; set; }
        public string Created_at { get; set; }
        public string Updated_at { get; set; }
        public string Deleted_at { get; set; }
        public string Route { get; set; }

        // Field specific attributes
        public int Category { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int PrimitiveType { get; set; }
        public bool ValueIsUnique { get; set; }
        public bool ValueIsRequired { get; set; }
        
        //Extra attributes
        public string Parent { get; set; }
    }
    public class PrimitiveType
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public string Csharp_name { get; set; }
        public string Js_name { get; set; }
        public bool Quantity_enabled { get; set; }
    }
    public class Phase
    {
        public string Name { get; set; }
        public string Slug { get; set; }
    }
    public class Keys
    {
        public const string Phase = "_ph";
        public const string Type = "_tp";
        public const string User = "_us";
        public const string Version = "_vr";
        public const string FormName = "_fn";

        public static bool ValidKey(string k)
        {
            return (k != Phase && k != Type && k != User && k != Version && k != FormName);
        }
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
