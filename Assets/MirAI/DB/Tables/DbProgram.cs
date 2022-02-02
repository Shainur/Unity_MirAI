﻿using System.Data;
using System.Text;
using Assets.MirAI.DB.TableDefs;

namespace Assets.MirAI.DB.Tables {
    public class DbProgram : Program, IDbEntity {

        public string GetDeleteCommandSuffix() {
            return " WHERE Id = '" + Id + "';";
        }

        public string GetInsertCommandSuffix() {
            return " (Name) VALUES ('" + Name + "');";
        }

        public string GetUpdateCommandSuffix() {
            return " SET Name = '" + Name + "' WHERE Id = '" + Id + "';";
        }

        public void SetData(IDataRecord data) {
            var count = data.FieldCount;
            if (count != 2) return;
            Id = data.GetInt32(0);
            Name = (string)data.GetString(1);
        }

        public override string ToString() {
            StringBuilder ret = new StringBuilder($"Id={Id,-5} Name={Name}");
            return ret.ToString();
        }
    }
}