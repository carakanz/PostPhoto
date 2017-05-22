using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace PostPhoto
{
    public class SizePhotoTable : TableEntity
    {
        public SizePhotoTable(string Name, int SizeX, int SizeY, DateTime When)
        {
            this.Name = Name;
            this.SizeX = SizeX;
            this.SizeY = SizeY;
            this.When = When;
            PartitionKey = string.Format("{0}.{1}", When.Year, When.Month);
            RowKey = Name;
        }
        public SizePhotoTable() { }

        public string Name { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public DateTime When { get; set; }
    }
    static class TableRecord //функции для сохранения результатов в cloud
    {
        private struct ParamentRecordTask
        {
            public string Name;
            public int SizeX;
            public int SizeY;
            public DateTime When;
        }

        public static void Record(string Name, int SizeX, int SizeY, DateTime When) // сохранить в таблицу {имя сохранённого файла, size x, size y, время запроса}
        {
            ParamentRecordTask parametr = new ParamentRecordTask();
            parametr.Name = Name;
            parametr.SizeX = SizeX;
            parametr.SizeY = SizeY;
            parametr.When = When;
            new Thread(TableRecord.RecordThread).Start(parametr);
            
        }

        private static void RecordThread(object parametr)// сохранить в таблицу. Тип parametr - ParamentRecordTask
        {
            ParamentRecordTask tempParametr = (ParamentRecordTask)parametr;
            try
            {
                if (CloudContainer.Table == null)
                {
                    CloudContainer.connectToTable();
                }
                SizePhotoTable record = new SizePhotoTable(tempParametr.Name, tempParametr.SizeX, tempParametr.SizeY, tempParametr.When);
                CloudContainer.Table.Execute(TableOperation.Insert(record));
            }
            catch (Exception exception)
            {
                Trace.TraceError("Can not write to table store. " + exception);
            }
        }

        public static IEnumerable<SizePhotoTable> GetDataAfter(DateTime dateTime) // возвращает все записи таблицы после даты dateTime
        {
            try
            {
                if (CloudContainer.Table == null)
                {
                    CloudContainer.connectToTable();
                }
                var q = from element in CloudContainer.Table.CreateQuery<SizePhotoTable>()
                        where element.When >= dateTime
                        select element;
                return q.ToArray();
            }
            catch (Exception exception)
            {
                Trace.TraceError("Can not read from table store. " + exception);
                return null;
            }
        }
    }

}