using Betkeeper.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Betkeeper.Models
{
    [Table("ErrorLog")]
    public class ErrorLog
    {
        [Column("Application")]
        public string Application { get; set; }

        [Column("StackTrace")]
        public string StackTrace { get; set; }

        [Column("Message")]
        public string Message { get; set; }

        [Column("Url")]
        public string Url { get; set; }

        [Key]
        [Column("ErrorLogId")]
        public int ErrorLogId { get; set; }

        [Column("Time")]
        public DateTime Time { get; set; }
    }

    public class ErrorLogRepository
    {
        private readonly BetkeeperDataContext _context;

        public ErrorLogRepository()
        {
            _context = new BetkeeperDataContext(Settings.OptionsBuilder);
        }

        public void AddError(ErrorLog errorLog)
        {
            if (errorLog.Time == default)
            {
                throw new Exception("No time given for error");
            }

            _context.ErrorLog.Add(errorLog);
            _context.SaveChanges();
        }
    }
}
