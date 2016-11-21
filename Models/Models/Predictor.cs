using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Prediction
    {
        public DateTime Date { get; internal set; }
        public decimal Amount { get; internal set; }
        public Category Category { get; internal set; }
    }

    public static class Predictor
    {
        /// <summary>
        /// Provides a sorted list of predictions for a given month.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static List<Prediction> Predict (DateTime date)
        {
            List<Prediction> predictions = new List<Prediction>();
            DateTime today = DateTime.Today;

            foreach (BudgetRecord record in Core.Instance.GetRecords(date.Year, date.Month))
            {
                switch (record.Type)
                {
                    case BudgetType.Monthly:
                        // TODO
                        break;
                    case BudgetType.Point:
                        DateTime recordDay = new DateTime(record.Year, record.Month, record.OnDay);
                        // prediction is valid only if it is on a later date than today.
                        if (recordDay > today)
                        {
                            predictions.Add(new Prediction
                            {
                                Date = recordDay,
                                Amount = record.Amount,
                                Category = record.Category
                            });
                        }
                        break;
                    case BudgetType.Daily:
                        // TODO
                        break;
                    case BudgetType.Weekly:
                        // TODO
                        break;
                    default:
                        break;
                }
            }

            predictions.OrderBy(pr => pr.Date);
            return predictions;
        }
    }
}
