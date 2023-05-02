using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThreadSample
{
    // 출처_1 : https://jacking75.github.io/csharp_asyncawait_lock/
    // 출처_2 : https://freeprog.tistory.com/469

    

    public partial class Form1 : Form
    {
        bool isBlock = false;

        private object lockObject = new object();
        private static readonly AsyncLock s_lock = new AsyncLock();
        
        public Form1()
        {
            InitializeComponent();

            DataTable dt = new DataTable();

            dt.Columns.Add("선택", typeof(bool));
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("제목", typeof(string));
            dt.Columns.Add("구분", typeof(string));
            dt.Columns.Add("생성일", typeof(string));
            dt.Columns.Add("수정일", typeof(string));

            // 각각의 행에 내용을 입력합니다.
            dt.Rows.Add(false, "ID 1", "제목 1번", "사용중", "2019/03/11", "2019/03/18");
            dt.Rows.Add(false, "ID 2", "제목 2번", "미사용", "2019/03/12", "2019/03/18");
            dt.Rows.Add(false, "ID 3", "제목 3번", "미사용", "2019/03/13", "2019/03/18");
            dt.Rows.Add(false, "ID 4", "제목 4번", "사용중", "2019/03/14", "2019/03/18");

            // 값들이 입력된 테이블을 DataGridView에 입력합니다.
            dataGridView1.DataSource = dt;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_Tick);

            timer.Start();
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            // no UI Block
            // no UI Delay

            await Task.Factory.StartNew(async () =>
            {
                /*
                if (isBlock)
                {
                    return;
                }
                else
                {
                    isBlock = true;

                    System.Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}] timer_Tick() Call_Start!");

                    //Thread.Sleep(5000);
                    await Task.Delay(5000);

                    System.Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}] timer_Tick() Call_End!");

                    isBlock = false;
                }
                */


                //lock (lockObject)
                using (await s_lock.LockAsync())
                {
                    System.Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}] timer_Tick() Call_Start!");

                    //Thread.Sleep(5000);
                    await Task.Delay(5000);
                    //Task.Delay(5000);

                    System.Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd hh:mm:ss}] timer_Tick() Call_End!");
                }

                
            });
        }
    }

    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            return new Handler(_semaphore);
        }

        private sealed class Handler : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private bool _disposed = false;

            public Handler(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _semaphore.Release();
                    _disposed = true;
                }
            }
        }
    }
}
