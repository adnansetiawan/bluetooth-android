using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace blutrack
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView mStatusTv;
        private Button mActivateBtn;
        private Button mPairedBtn;
        private Button mScanBtn;
        private BluetoothBroadcastReceiver broadcastReceiver;
        private ProgressDialog mProgressDlg;

        private List<BluetoothDevice> mDeviceList = new List<BluetoothDevice>();

        private BluetoothAdapter mBluetoothAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            mStatusTv = (TextView)FindViewById(Resource.Id.tv_status);
            mActivateBtn = (Button)FindViewById(Resource.Id.btn_enable);
            mPairedBtn = (Button)FindViewById(Resource.Id.btn_view_paired);
            mScanBtn = (Button)FindViewById(Resource.Id.btn_scan);

            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            mProgressDlg = new ProgressDialog(this);

            mProgressDlg.SetMessage("Scanning...");
            mProgressDlg.SetCancelable(false);
            IntentFilter filter = new IntentFilter();

            filter.AddAction(BluetoothAdapter.ActionStateChanged);
            filter.AddAction(BluetoothDevice.ActionFound);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            broadcastReceiver = new BluetoothBroadcastReceiver(() =>
            {
                showToast("Enabled");
                showEnabled();
            }, () => { mProgressDlg.Show(); }, mDeviceList);

            RegisterReceiver(broadcastReceiver, filter);

            if (mBluetoothAdapter == null)
            {
                showUnsupported();
            }
            else
            {
                mPairedBtn.Click += (sender, e) =>
                {
                    var pairedDevices = mBluetoothAdapter.BondedDevices;
                    if (pairedDevices == null || pairedDevices.Count == 0)
                    {
                        showToast("No Paired Devices Found");
                    }
                    else
                    {
                        IList<IParcelable> list = new List<IParcelable>();
                        foreach (var device in pairedDevices)
                        {
                            list.Add(device);
                        }
                        Intent intent = new Intent(this, typeof(DeviceListActivity));

                        intent.PutParcelableArrayListExtra("device.list", list);

                        StartActivity(intent);
                    }

                };
                mScanBtn.Click += (sender, e) =>
                {
                    if (mBluetoothAdapter.IsDiscovering)
                    {
                        // cancel the discovery if it has already started
                        mBluetoothAdapter.CancelDiscovery();
                    }

                    if (mBluetoothAdapter.StartDiscovery())
                    {
                        // bluetooth has started discovery
                    }
                };
                mActivateBtn.Click += (sender, e) =>
                {
                    if (mBluetoothAdapter.IsEnabled)
                    {
                        mBluetoothAdapter.Disable();

                        showDisabled();
                    }
                    else
                    {
                        Intent intent = new Intent(BluetoothAdapter.ActionRequestEnable);

                        StartActivityForResult(intent, 1000);
                    }

                };

                if (mBluetoothAdapter.IsEnabled)
                {
                    showEnabled();
                }
                else
                {
                    showDisabled();
                }



            }

        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }
        protected override void OnPause()
        {
            if (mBluetoothAdapter != null)
            {
                if (mBluetoothAdapter.IsDiscovering)
                {
                    mBluetoothAdapter.CancelDiscovery();
                }
            }
            base.OnPause();
        }
        protected override void OnDestroy()
        {
            UnregisterReceiver(broadcastReceiver);
            base.OnDestroy();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private void showEnabled()
        {
            mStatusTv.Text = "Bluetooth is On";
            //mStatusTv.SetTextColor(Color.BLUE);

            mActivateBtn.Text = "Disable";
            mActivateBtn.Enabled = true;

            mPairedBtn.Enabled = true;
            mScanBtn.Enabled = true;
        }

        private void showDisabled()
        {
            mStatusTv.Text = "Bluetooth is Off";
            //mStatusTv.setTextColor(Color.RED);

            mActivateBtn.Text = "Enable";
            mActivateBtn.Enabled = true;

            mPairedBtn.Enabled = false;
            mScanBtn.Enabled = false;
        }

        private void showUnsupported()
        {
            mStatusTv.Text = "Bluetooth is unsupported by this device";

            mActivateBtn.Text = "Enable";
            mActivateBtn.Enabled = false;

            mPairedBtn.Enabled = false;
            mScanBtn.Enabled = false;
        }

        private void showToast(String message)
        {
            Toast.MakeText(ApplicationContext, message, ToastLength.Short).Show();
        }


    }

    public class BluetoothBroadcastReceiver : BroadcastReceiver
    {
        private Action _actionStateOn { get; set; }
        private Action _progressShow { get; set; }

        private List<BluetoothDevice> _deviceList;


        public BluetoothBroadcastReceiver(Action actionStateOn, Action progressShow, List<BluetoothDevice> deviceList)
        {
            _actionStateOn = actionStateOn;
            _deviceList = deviceList;
            _progressShow = progressShow;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;

            if (BluetoothAdapter.ActionStateChanged.Equals(action))
            {
                int state = intent.GetIntExtra(BluetoothAdapter.ExtraState, BluetoothAdapter.Error);
                if (state == 12)
                {
                    _actionStateOn();
                }



            }
            else if (BluetoothAdapter.ActionDiscoveryStarted.Equals(action))
            {
                _deviceList = new List<BluetoothDevice>();

                _progressShow();
            }

        }

    }



}





