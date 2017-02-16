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
        /// Enumerates every day of the month.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        private static IEnumerable<DateTime> EachDay(int year, int month)
        {
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                yield return new DateTime(year, month, i);
            }
        }

        /// <summary>
        /// Provides a sorted list of predictions for a given month.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public static List<Prediction> Predict (DateTime date)
        {
            List<Prediction> predictions = new List<Prediction>();
            DateTime today = DateTime.Today;
            int daysThisMonth = DateTime.DaysInMonth(date.Year, date.Month);

            foreach (BudgetRecord record in Core.Instance.GetRecords(date.Year, date.Month))
            {
                switch (record.Type)
                {
                    case BudgetType.Monthly:
                        // One time cumulative prediction. Prediction will be set to the last
                        // day of the month. Prediction depends on other budget records.
                        // Since transactions do not have direct reference to 
                        // budget records, there is no clear way to make 100% 
                        // exact prediction. Prediction amount is the minimum
                        // of what is left to spend (based on all budget records for that category and
                        // all transactions) and actual budget record planned amount.

                        // Fetch spending for a correspondig category to see cumulative effect
                        // of all budget records and transactions.
                        Spending spending =
                            Core.Instance.GetSpendings(date.Year, date.Month)
                            .Where((s) => s.Category == record.Category)
                            .DefaultIfEmpty(new Spending {Budget = 0m, Value = 0m})
                            .First();
                        // Spending are abs values, have to make budget record abs too.
                        decimal absAmount = Math.Abs(record.Amount);

                        decimal toSpend = spending.Budget - spending.Value;
                        DateTime rday = new DateTime(record.Year, record.Month, daysThisMonth);
                        // if we have something left in the total budget.
                        // prediction is valid only if it is on a later date than today.
                        if (toSpend > 0 && rday > today)
                        {
                            predictions.Add(new Prediction
                            {
                                Date = rday,
                                Amount = Math.Min(toSpend, absAmount) * Math.Sign(record.Amount),
                                Category = record.Category
                            });
                        }
                        break;
                    case BudgetType.Point:
                        // One time prediction. Will be generated only on the specified day.
                        DateTime rDay = new DateTime(record.Year, record.Month, record.OnDay);
                        // prediction is valid only if it is on a later date than today.
                        if (rDay > today)
                        {
                            predictions.Add(new Prediction
                            {
                                Date = rDay,
                                Amount = record.Amount,
                                Category = record.Category
                            });
                        }
                        break;
                    case BudgetType.Daily:
                        // Generates predictions for every day of month with a fraction
                        // of total Amount.
                        decimal dailyAmount = record.Amount / daysThisMonth;
                        // prediction is valid only if it is on a later date than today.
                        foreach (DateTime day in EachDay(record.Year, record.Month))
                        {
                            if (day > today)
                            {
                                predictions.Add(new Prediction
                                {
                                    Date = day,
                                    Amount = dailyAmount,
                                    Category = record.Category
                                });
                            }
                        }
                        break;
                    case BudgetType.Weekly:
                        // Generates predictions for every specified day of the week
                        // with a fraction of total Amount.
                        List<DateTime> budgetDays =
                            EachDay(record.Year, record.Month)
                            .Where(d => (int)d.DayOfWeek == record.OnDay)
                            .ToList();
                        decimal weeklyAmount = record.Amount / budgetDays.Count;

                        foreach (DateTime day in budgetDays)
                        {
                            if (day > today)
                            {
                                predictions.Add(new Prediction
                                {
                                    Date = day,
                                    Amount = weeklyAmount,
                                    Category = record.Category
                                });
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return predictions.OrderBy(pr => pr.Date).ToList();
        }
    }
}
