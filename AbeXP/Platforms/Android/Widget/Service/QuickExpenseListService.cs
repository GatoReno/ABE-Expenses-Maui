using AbeXP.Models;
using AbeXP.UseCases.Interfaces;
using Android.App;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Widget.RemoteViewsService;

namespace AbeXP.Platforms.Android.Widget.Service
{
    internal class QuickExpenseListService
    {
        [Service(Permission = "android.permission.BIND_REMOTEVIEWS")]
        public class WidgetListService : RemoteViewsService
        {
            public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
            {
                return new WidgetListFactory(this.ApplicationContext, intent);
            }

        }

        public class WidgetListFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
        {
            private readonly Context _context;

            private readonly IGetTransactionsUseCase _getExpendituresUseCase;
            private List<TransactionItem> _items = new();

            public WidgetListFactory(Context context, Intent intent)
            {
                _context = context;

                _getExpendituresUseCase = MauiApplication.Current.Services.GetService<IGetTransactionsUseCase>();
            }

            public void OnCreate()
            {
                //LoadData();
            }

            public void OnDataSetChanged()
            {
                LoadData().Wait();
            }

            public void OnDestroy() => _items.Clear();

            public int Count => _items.Count;

            public RemoteViews GetViewAt(int position)
            {
                var rv = new RemoteViews(_context.PackageName, Resource.Layout.quickexpense_list_item);
                var item = _items[position];

                // Type
                rv.SetTextViewText(Resource.Id.txtType, item.TypeDescription);

                // Description
                rv.SetTextViewText(Resource.Id.txtDescription, item.Description);

                // Amount
                rv.SetTextViewText(Resource.Id.txtAmount, $"{item.Amount:C}");

                // Date
                rv.SetTextViewText(Resource.Id.txtDate, item.Date.ToString("yyyy-MM-dd"));

                // Payment type
                rv.SetTextViewText(Resource.Id.txtPaymentType, item.PaymentMethod);

                return rv;
            }

            public RemoteViews LoadingView => null;
            public int ViewTypeCount => 1;
            public long GetItemId(int position) => position;
            public bool HasStableIds => true;

            private async Task LoadData()
            {
                try
                {
                    var itemsResult =  await _getExpendituresUseCase.ExecuteAsync(new TransactionRequest
                    {
                        LimitTo = 30
                    });

                    _items = new List<TransactionItem>(itemsResult.Payload);
                }
                catch (Exception ex)
                {
                    App.Alert.ShowAlert("Error", "Could not load transactions.");
                }
            }
        }
    }
}
