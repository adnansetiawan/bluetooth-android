using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace blutrack
{
    public class DeviceListAdapter : BaseAdapter
    {
        private LayoutInflater mInflater;
        private List<BluetoothDevice> mData;
        private OnPairButtonClickListener mListener;

        public DeviceListAdapter(Context context)
        {
            mInflater = LayoutInflater.From(context);
        }
        public void SetData(List<BluetoothDevice> data)
        {
            mData = data;
        }

        public void SetListener(OnPairButtonClickListener listener)
        {
            mListener = listener;
        }
        public override int Count
        {
            get
            {
                return mData == null ? 0 : mData.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ViewHolder holder;

            if (convertView == null)
            {
                convertView = mInflater.Inflate(Resource.Layout.list_item_devices, null);

                holder = new ViewHolder();

                holder.nameTv = (TextView)convertView.FindViewById(Resource.Id.tv_name);
                holder.addressTv = (TextView)convertView.FindViewById(Resource.Id.tv_address);
                holder.pairBtn = (Button)convertView.FindViewById(Resource.Id.btn_pair);

                convertView.Tag = holder;
            }
            else
            {
                holder = (ViewHolder)convertView.Tag;
            }
            BluetoothDevice device = mData[position];

            holder.nameTv.Text = device.Name;
            holder.addressTv.Text = device.Address;

            return convertView;

        }




        class ViewHolder : Java.Lang.Object
        {
            public TextView nameTv;
            public TextView addressTv;
            public TextView pairBtn;
        }

        public interface OnPairButtonClickListener
        {
            public abstract void onPairButtonClick(int position);
        }
    }
}
