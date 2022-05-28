using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Users;

namespace ProductionManagementSystem.Core.Models.Logs
{
    [Table("Logs")]
    public class Log : BaseEntity
    {
        
        [Display(Name = "Дата и время")]
        public DateTime DateTime { get; set; }
        
        [Display(Name = "Сообщение")]
        public string Message { get; set; }
        
        [Display(Name = "Пользователь")]
        public User User { get; set; }        
        public string UserId { get; set; }
        public int ItemId { get; set; }
        public LogsItemType ItemType { get; set; }

        public bool HasChanges => _hasChanges;
        private bool _hasChanges = false;
        
        public Log()
        {
            DateTime = DateTime.Now;
        }

        public Log(LogsItemType itemType, int itemId) : base()
        {
            DateTime = DateTime.Now;
            ItemId = itemId;
            ItemType = itemType;
        }
        
        public void AddMessage(string Message)
        {
            this.Message = this.Message + Message + "<br />";
        }

        public void AddMessageParameterSet(string ParameterName, string Value)
        {
            this.Message = this.Message + String.Format("<b>{0}</b> = '<i>{1}</i>'.<br />", ParameterName, Value);
        }


        public void AddMessageParameterChange(string ParameterName, string OldValue, string NewValue)
        {
            if (OldValue != NewValue)
            {
                this.Message = this.Message + String.Format("<b>{0}</b> было изменено <b>c</b> '<i>{1}</i>' <b>на</b> '<i>{2}</i>'.<br />", ParameterName, OldValue, NewValue);
                this._hasChanges = true;
            }
        }
    }

    public enum LogsItemType
    {
        Montage,
        Design,
        Device,
        Task,
        Order,
        DesignSupplyRequest,
        MontageSupplyRequest
    }
}