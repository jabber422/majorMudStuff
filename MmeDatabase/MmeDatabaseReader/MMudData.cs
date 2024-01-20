﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMudObjects;
using System.Data.OleDb;
using System.IO;

namespace MmeDatabaseReader
{
    public static class MMudData
    {
        static string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                   @"Data Source=master.mdb;" +
                   "Persist Security Info=True;" +
                   "Jet OLEDB:Database Password=myPassword;";

        public static Item GetItem(Item item)
        {
            try
            {
                var d = Directory.GetCurrentDirectory();
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();

                    string getDbName = @"" +
                        "SELECT *" +
                        "FROM   Items " +
                        $"WHERE(Name = '{item.Name}')";


                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            DataRow row = myDataTable.Rows[0];
                            var type = (EnumItemType)Enum.Parse(typeof(EnumItemType), row["ItemType"].ToString());
                            //switch (type)
                            //{
                            //    case EnumItemType.Armor:
                            //        item = new Armor(item);

                            //        break;
                            //    case EnumItemType.Weapon:
                            //        item = new Weapon(item);

                            //        (item as Weapon)
                            //        break;
                            //    case EnumItemType.Food:
                            //    case EnumItemType.Potion:
                            //    case EnumItemType.Light:
                            //    case EnumItemType.Key:
                            //    case EnumItemType.Chest:
                            //    case EnumItemType.Scroll:
                            //    case EnumItemType.Gem:
                            //        break;
                            //}
                            item.Id = int.Parse(row["Number"].ToString());
                            item.Limit = int.Parse(row["Limit"].ToString());
                            item.Encum = int.Parse(row["Encum"].ToString());
                            item.Type = type;
                            item.Price = int.Parse(row["Price"].ToString());
                            item.Currency = int.Parse(row["Currency"].ToString());
                            item.MinDamage = int.Parse(row["Min"].ToString());
                            item.MaxDamage = int.Parse(row["Max"].ToString());
                            item.AC = int.Parse(row["ArmourClass"].ToString());
                            item.DR = int.Parse(row["DamageResist"].ToString());
                            item.WeaponType = (EnumWeaponType)Enum.Parse(typeof(EnumWeaponType), row["WeaponType"].ToString());
                            item.ArmorType = (EnumArmorType)Enum.Parse(typeof(EnumArmorType), row["ArmourType"].ToString());
                            item.EquipmentSlot = (EnumEquipmentSlot)Enum.Parse(typeof(EnumEquipmentSlot), row["Worn"].ToString());
                            item.Accuracy = int.Parse(row["Accy"].ToString());
                            item.Gettable = int.Parse(row["Gettable"].ToString()) == 1 ? true : false;
                            item.Strength = int.Parse(row["StrReq"].ToString());
                            item.Speed = int.Parse(row["Speed"].ToString());


                            for (int i = 0; i < 20; i++)
                            {
                                var abil = new ItemAbility();
                                abil.Abililty = int.Parse(row[$"Abil-{i}"].ToString());
                                abil.Value = int.Parse(row[$"AbilVal-{i}"].ToString());
                                item.Abilities.Add(abil);
                            }

                            return item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return item;
            }
        }
    

        public static Entity GetNpc(Entity e)
        {
            try
            {
                var d = Directory.GetCurrentDirectory();
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();

                    string getDbName = @"" +
                        "SELECT *" +
                        "FROM   Monsters " +
                        $"WHERE(Name = '{e.Name}')";


                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            DataRow row = myDataTable.Rows[0];
                            NPC npc = new NPC(e);
                            npc.Id = row["Number"].ToString();
                            npc.Exp = int.Parse(row["EXP"].ToString());
                            npc.Regen = int.Parse(row["RegenTime"].ToString());
                            npc.Type = (EnumNpcType)Enum.Parse(typeof(EnumNpcType), row["Type"].ToString());
                            npc.Alignment = (EnumNpcAlignment)Enum.Parse(typeof(EnumNpcAlignment), row["Align"].ToString());
                            npc.Health = int.Parse(row["HP"].ToString());
                            npc.HealthRegen = int.Parse(row["HPRegen"].ToString());
                            npc.AC = int.Parse(row["ArmourClass"].ToString());
                            npc.DR = int.Parse(row["DamageResist"].ToString());
                            npc.MR = int.Parse(row["MagicRes"].ToString());
                            npc.FollowPercentage = int.Parse(row["Follow%"].ToString());
                            npc.CharmLevel = int.Parse(row["CharmLVL"].ToString());

                            return npc;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return e;
            }
        }
    }
}