using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views.Animations;

namespace ButterflyTest
{
    [Activity(Label = "ButterflyTest", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        const string TAG = "调试信息";

        // 记录蝴蝶ImageView当前的位置
        private float curX = 0;
        private float curY = 30;
        // 记录蝴蝶ImageView下一个位置的坐标
        float nextX = 0;
        float nextY = 0;

        /// <summary>
        /// 显示蝴蝶的ImageView控件
        /// </summary>
        ImageView imageView;

        /// <summary>
        /// 处理自定义消息的类对象
        /// </summary>
        static MyHandler mh;

        /// <summary>
        /// Drawable animation可以加载Drawable资源实现帧动画
        /// </summary>
        AnimationDrawable butterfly;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
            
            imageView = (ImageView)FindViewById(Resource.Id.img_butterfly);
            mh = new MyHandler(this);
            if (imageView.Background == null)
            {
                Log.Debug(TAG, "Background为空!");
            }
            butterfly = (AnimationDrawable)imageView.Drawable;
            if (imageView.Drawable == null)
            {
                Log.Debug(TAG, "Drawable为空!");
            }

            imageView.Click += imgOnClick;
        }

        /// <summary>
        /// 自定义Handle类,用于处理当前活动接受的消息
        /// </summary>
        class MyHandler : Handler
        {
            /// <summary>
            /// 获取当前活动的属性值
            /// </summary>
            private MainActivity m;

            /// <summary>
            /// 随机数生成类对象
            /// </summary>
            private Random r = new Random();

            /// <summary>
            /// 为当前活动专门创建的构造方法,获取当前活动的引用
            /// </summary>
            /// <param name="ma"></param>
            public MyHandler(MainActivity ma) : base()
            {
                m = ma;
            }


            public override void HandleMessage(Message msg)
            {
                base.HandleMessage(msg);
                if (msg.What == 0x123)
                {
                    // 横向上一直向右飞
                    if (m.nextX > 320)
                    {
                        m.curX = m.nextX = 0;
                    }
                    else { m.nextX += 8; }
                }
                // 纵向上可以随机上下
                m.nextY = m.curY + (float)(r.NextDouble() * 10 - 5);
                // 设置显示蝴蝶的ImageView发生位移改变
                TranslateAnimation anim = new TranslateAnimation(m.curX, m.nextX, m.curY, m.nextY);
                m.curX = m.nextX;
                m.curY = m.nextY;
                anim.Duration = 200;
                // 开始位移动画
                m.imageView.StartAnimation(anim);
            }
        }



        /// <summary>
        /// ImageView的点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void imgOnClick(object sender, EventArgs e)
        {
            Log.Debug(TAG,"点击事件开始执行");
            // 开始播放蝴蝶振翅的逐帧动画
            if (butterfly == null)
            {
                Log.Debug(TAG, "butterfly为空!");
            }
            butterfly.Start();
            Log.Debug(TAG, "定时任务开始执行");
            // 通过定时器控制每0.2秒运行一次TranslateAnimation动画
            new Java.Util.Timer().Schedule(new MyTimeTask(), 0, 200);
            Log.Debug(TAG, "定时任务执行完成");
        }


        /// <summary>
        /// 自定义定时任务类,开启一个线程,在线程中发送handle消息
        /// </summary>
        class MyTimeTask : Java.Util.TimerTask
        {
            public override void Run()
            {
                mh.SendEmptyMessage(0x123);
            }
        }

    }
}

