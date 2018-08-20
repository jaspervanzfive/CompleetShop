using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Prism.Mvvm;

namespace CompleetKassa.DataValidation
{
	public abstract class ModelValidationBase : BindableBase, IDataErrorInfo
	{
		private Dictionary<string, Binder> ruleMap = new Dictionary<string, Binder>();

		public void AddRule<T>(Expression<Func<T>> expression, Func<bool> ruleDelegate, string errorMessage)
		{
			var name = GetPropertyName(expression);

			ruleMap.Add(name, new Binder(ruleDelegate, errorMessage));
		}

		protected override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			Binder binder;
			if (ruleMap.TryGetValue(propertyName, out binder) == true)
			{
				binder.IsDirty = true;
			}

			return base.SetProperty<T>(ref storage, value, propertyName);
		}

		public bool HasErrors
		{
			get
			{
				var values = ruleMap.Values.ToList();
				values.ForEach(b => b.Update());

				return values.Any(b => b.HasError);
			}
		}

		public string Error
		{
			get
			{
				var errors = from b in ruleMap.Values where b.HasError select b.Error;

				return string.Join("\n", errors);
			}
		}

		public string this[string columnName]
		{
			get
			{
				if (ruleMap.ContainsKey(columnName))
				{
					ruleMap[columnName].Update();
					return ruleMap[columnName].Error;
				}
				return null;
			}
		}

		protected static string GetPropertyName<T>(Expression<Func<T>> expression)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");

			Expression body = expression.Body;
			MemberExpression memberExpression = body as MemberExpression;
			if (memberExpression == null)
			{
				memberExpression = (MemberExpression)((UnaryExpression)body).Operand;
			}
			return memberExpression.Member.Name;
		}

		private class Binder
		{
			private readonly Func<bool> ruleDelegate;
			private readonly string message;

			internal Binder(Func<bool> ruleDelegate, string message)
			{
				this.ruleDelegate = ruleDelegate;
				this.message = message;

				IsDirty = true;
			}

			internal string Error { get; set; }
			internal bool HasError { get; set; }

			internal bool IsDirty { get; set; }

			internal void Update()
			{
				if (!IsDirty)
					return;

				Error = null;
				HasError = false;
				try
				{
					if (!ruleDelegate())
					{
						Error = message;
						HasError = true;
					}
				}
				catch (Exception e)
				{
					Error = e.Message;
					HasError = true;
				}
			}
		}
	}
}
