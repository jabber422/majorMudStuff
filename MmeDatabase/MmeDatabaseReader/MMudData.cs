using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMudObjects;
using System.Data.OleDb;
using System.IO;
using System.Runtime.CompilerServices;

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


                            return RowToItem(row, item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Item error occurred: " + ex.Message);
                return item;
            }
        }

        public static Spell GetSpell(Spell spell)
        {
            try
            {
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();

                    string getDbName = @"" +
                        "SELECT *" +
                        "FROM   Spells " +
                        $"WHERE(Name = '{spell.Name}')";


                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            return new Spell(myDataTable.Rows[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Spell error occurred: " + ex.Message);
                return spell;
            }
        }

        public static EnumMagicType GetMagery(Player player)
        {
            try {
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString)) {
                    myConnection.Open();
                    string getDbName = @"" +
                        "SELECT *" +
                        "FROM   Classes " +
                        $"WHERE(Name = '{player.Stats.Class}')";

                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection)) {
                        using (OleDbDataReader reader = cmd.ExecuteReader()) {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            DataRow row = myDataTable.Rows[0];
                            return (EnumMagicType)Enum.Parse(typeof(EnumMagicType), row["MageryType"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMagery error occurred: " + ex.Message);
                return EnumMagicType.None;
            }
        }

        public static int GetMageryLevel(Player player)
        {
            try
            {
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();
                    string getDbName = @"" +
                        "SELECT *" +
                        "FROM   Classes " +
                        $"WHERE(Name = '{player.Stats.Class}')";

                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            DataRow row = myDataTable.Rows[0];
                            return int.Parse(row["MageryLVL"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMagery error occurred: " + ex.Message);
                return 0;
            }
        }

        public static Entity GetNpc(Entity e)
        {
            if (e.Name == "You" || e.Name == "you") return e;
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

                            npc.BaddieFlag = true;
                            if (npc.Alignment == EnumNpcAlignment.GOOD || npc.Alignment == EnumNpcAlignment.L_GOOD)
                            {
                                npc.BaddieFlag = false;
                            }

                            return npc;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("NPC error occurred: " + ex.Message);
                return e;
            }
        }

        public static List<Spell> GetSpellList(Player player)
        {
            var magictype = MMudData.GetMagery(player);
            var magiclevel = MMudData.GetMageryLevel(player);

            List<Spell> spells = new List<Spell>();
            try
            {
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();

                    string getDbName = @"" +
                        "SELECT * " +
                        "FROM Spells " +
                        $"WHERE(Magery = {(int)magictype} AND MageryLVL > 0 AND MageryLVL <= {magiclevel} and ReqLevel <= {player.Stats.Level})";

                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            foreach (DataRow row in myDataTable.Rows)
                            {
                                spells.Add(new Spell(row));
                            }

                            return spells;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Spell error occurred: " + ex.Message);
                return spells;
            }
        }

        public static List<Item> GetTorches()
        {
            List<Item> items = new List<Item>();
            try
            {
                var d = Directory.GetCurrentDirectory();
                using (OleDbConnection myConnection = new OleDbConnection(myConnectionString))
                {
                    myConnection.Open();

                    string getDbName = @"" +
                        "SELECT *" +
                        "FROM   Items " +
                        $"WHERE(ItemType = '6')";


                    using (OleDbCommand cmd = new OleDbCommand(getDbName, myConnection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable myDataTable = new DataTable();
                            myDataTable.Load(reader);

                            foreach (DataRow row in myDataTable.Rows)
                            {
                                items.Add(RowToItem(row, new Item(row["Name"].ToString())));
                            }
                        }
                    }
                }
            }
            catch (Exception ex) 
            { Console.WriteLine(ex.Message); return null; }
            return items;
        }

        private static Item RowToItem(DataRow row, Item item)
        {
            //
            var type = (EnumItemType)Enum.Parse(typeof(EnumItemType), row["ItemType"].ToString());

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

            item.ObtainedFrom = row[$"Obtained From"].ToString();
            return item;
        }
    }
}
