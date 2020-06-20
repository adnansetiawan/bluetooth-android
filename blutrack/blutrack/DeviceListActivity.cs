
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace blutrack
{
    [Activity(Label = "DeviceListActivity")]
    public class DeviceListActivity : Activity
    {
        private ListView mListView;
        private DeviceListAdapter mAdapter;
        private List<BluetoothDevice> mDeviceList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_paired_devices);
            try
            {
                mDeviceList = new List<BluetoothDevice>();
                var devices = Intent.Extras.GetParcelableArrayList("device.list");
                foreach (var item in devices)
                {
                    mDeviceList.Add(item as BluetoothDevice);
                }
            }
            catch (Exception ex)
            {
            }
            mListView = (ListView)FindViewById(Resource.Id.lv_paired);

            mAdapter = new DeviceListAdapter(this);

            mAdapter.SetData(mDeviceList);
            mAdapter.SetListener(new OnPairButtonClickListener(mDeviceList));
            mListView.Adapter = mAdapter;

            //RegisterReceiver(mPairReceiver, new IntentFilter(BluetoothDevice.ACTION_BOND_STATE_CHANGED));
        }
    }
    class OnPairButtonClickListener : DeviceListAdapter.OnPairButtonClickListener
    {
        private List<BluetoothDevice> _mDeviceList;
        public OnPairButtonClickListener(List<BluetoothDevice> mDeviceList)
        {
            _mDeviceList = mDeviceList;
        }
        public void onPairButtonClick(int position)
        {
            BluetoothDevice device = _mDeviceList[position];

            /*if (device.BondState == BluetoothDevice.BONDE)
            {
                //  unpairDevice(device);
            }
            else
            {
                //showToast("Pairing...");

                //pairDevice(device);
            }*/
        }
    }
}
