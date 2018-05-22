using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Playground.EFEntityGenerator
{
   

    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public string DbTypeName { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsNullable { get; set; }

        public string DotNetTypeName
        {
            get
            {
                return EFEntityGenerator.DbDotNetTypeMapping.Where(p => p.Item1 == DbTypeName).First().Item2;
            }
        }

        //to-do: implement foreign key
    }

    public class EFEntityGenerator
    {
        public static List<Tuple<string, string>> DbDotNetTypeMapping = new List<Tuple<string, string>>()
    {
        new Tuple<string, string>("datetime","datetime"),
        new Tuple<string, string>("decimal","decimal"),
        new Tuple<string, string>("float","double"),
        new Tuple<string, string>("int","int"),
        new Tuple<string, string>("nvarchar","string"),
        new Tuple<string, string>("bit","bool"),
        new Tuple<string,string>("bigint","long"),
        new Tuple<string,string>("char","char"),
        new Tuple<string,string>("date","datetime"),
        new Tuple<string,string>("datetime","datetime"),
        new Tuple<string,string>("datetime2","datetime"),
        new Tuple<string,string>("money","decimal"),
        new Tuple<string,string>("nchar","char"),
        new Tuple<string,string>("ntext","string"),
        new Tuple<string,string>("smalldatetime","datetime"),
        new Tuple<string,string>("smallint","short"),
        new Tuple<string,string>("smallmoney","decimal"),
        new Tuple<string,string>("text","string"),
        new Tuple<string,string>("xml","string"),
        new Tuple<string,string>("tinyint","byte"),
        new Tuple<string,string>("uniqueidentifier","Guid"),
        //new Tuple<string,string>("varbinary",""),
        //new Tuple<string,string>("varchar",""),
        //new Tuple<string,string>("datetimeoffset",""),
        //new Tuple<string,string>("geography",""),
        //new Tuple<string,string>("geometry",""),
        //new Tuple<string,string>("hierarchyid",""),
        //new Tuple<string,string>("image",""),
        //new Tuple<string,string>("binary",""),
        //new Tuple<string,string>("time",""),
        //new Tuple<string,string>("timestamp",""),
        //new Tuple<string,string>("sql_variant",""),
        //new Tuple<string,string>("sysname",""),
          //new Tuple<string,string>("numeric",""),
        //new Tuple<string,string>("real",""),



    };
        public List<Table> GetTableAndViewList()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT  * FROM 
                                                     INFORMATION_SCHEMA.COLUMNS 
                                                     ORDER BY TABLE_NAME, ORDINAL_POSITION", con);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var dtAsEnumerable = dt.AsEnumerable().ToList();

            //to-do: handle multiple schema!
            List<Table> tables = dtAsEnumerable.Select(p => p["TABLE_SCHEMA"].ToString() + "." + p["TABLE_NAME"].ToString()).Distinct().Select(tableName => new Table()
            {
                Name = tableName.Split('.').Last(),
                Columns = dtAsEnumerable.Where(p => p["TABLE_SCHEMA"].ToString() + "." + p["TABLE_NAME"].ToString() == tableName).Select
                              (col =>
                              new Column()
                              {
                                  Name = col["COLUMN_NAME"].ToString(),
                                  DbTypeName = col["DATA_TYPE"].ToString(),
                                  IsNullable = Convert.ToBoolean(col["IS_NULLABLE"].ToString() == "YES")
                              }).ToList()

            }).ToList();

            return tables;
        }
    }



}

