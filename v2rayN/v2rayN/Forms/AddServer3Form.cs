﻿using System;
using System.Windows.Forms;
using v2rayN.Handler;
using v2rayN.Mode;

namespace v2rayN.Forms
{
    public partial class AddServer3Form : BaseForm
    {
        public int EditIndex { get; set; }
        VmessItem vmessItem = null;

        public AddServer3Form()
        {
            InitializeComponent();
            this.TopMost = true;
        }

        private void AddServer3Form_Load(object sender, EventArgs e)
        {
            if (EditIndex >= 0)
            {
                vmessItem = config.vmess[EditIndex];
                BindingServer();
            }
            else
            {
                vmessItem = new VmessItem();
                ClearServer();
            }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        private void BindingServer()
        {

            txtAddress.Text = vmessItem.address;
            txtPort.Text = vmessItem.port.ToString();
            txtId.Text = vmessItem.id;
            cmbSecurity.Text = vmessItem.security;
            txtRemarks.Text = vmessItem.remarks;
        }


        /// <summary>
        /// 清除设置
        /// </summary>
        private void ClearServer()
        {
            txtAddress.Text = "";
            txtPort.Text = "";
            txtId.Text = "";
            cmbSecurity.Text = Global.DefaultSecurity;
            txtRemarks.Text = "";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string address = txtAddress.Text;
            string port = txtPort.Text;
            string id = txtId.Text;
            string security = cmbSecurity.Text;
            string remarks = txtRemarks.Text;

            if (Utils.IsNullOrEmpty(address))
            {
                UI.Show("请填写服务器地址");
                return;
            }
            if (Utils.IsNullOrEmpty(port) || !Utils.IsNumberic(port))
            {
                UI.Show("请填写正确格式服务器端口");
                return;
            }
            if (Utils.IsNullOrEmpty(id))
            {
                UI.Show("请填写密码");
                return;
            }
            if (Utils.IsNullOrEmpty(security))
            {
                UI.Show("请选择加密方式");
                return;
            }

            vmessItem.address = address;
            vmessItem.port = Utils.ToInt(port);
            vmessItem.id = id;
            vmessItem.security = security;
            vmessItem.remarks = remarks;

            if (ConfigHandler.AddShadowsocksServer(ref config, vmessItem, EditIndex) == 0)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                UI.Show("操作失败，请检查重试");
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }


        #region 导入客户端/服务端配置

        /// <summary>
        /// 导入客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemImportClient_Click(object sender, EventArgs e)
        {
            MenuItemImport(1);
        }

        /// <summary>
        /// 导入服务端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemImportServer_Click(object sender, EventArgs e)
        {
            MenuItemImport(2);
        }

        private void MenuItemImport(int type)
        {
            //ClearServer();

            //OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.Multiselect = false;
            //fileDialog.Filter = "Config|*.json|所有文件|*.*";
            //if (fileDialog.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}
            //string fileName = fileDialog.FileName;
            //if (Utils.IsNullOrEmpty(fileName))
            //{
            //    return;
            //}
            //string msg;
            //VmessItem vmessItem;
            //if (type.Equals(1))
            //{
            //    vmessItem = V2rayConfigHandler.ImportFromClientConfig(fileName, out msg);
            //}
            //else
            //{
            //    vmessItem = V2rayConfigHandler.ImportFromServerConfig(fileName, out msg);
            //}
            //if (vmessItem == null)
            //{
            //    UI.Show(msg);
            //    return;
            //}

            //txtAddress.Text = vmessItem.address;
            //txtPort.Text = vmessItem.port.ToString();
            //txtId.Text = vmessItem.id;
            //txtAlterId.Text = vmessItem.alterId.ToString();
            //txtRemarks.Text = vmessItem.remarks;
            //cmbNetwork.Text = vmessItem.network;
            //cmbHeaderType.Text = vmessItem.headerType;
            //txtRequestHost.Text = vmessItem.requestHost;
            //cmbStreamSecurity.Text = vmessItem.streamSecurity;
        }

        /// <summary>
        /// 从剪贴板导入URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemImportClipboard_Click(object sender, EventArgs e)
        {
            ImportConfig();
        }

        private void ImportConfig()
        {
            ClearServer();

            string msg;
            VmessItem vmessItem = V2rayConfigHandler.ImportFromClipboardConfig(Utils.GetClipboardData(), out msg);
            if (vmessItem == null)
            {
                UI.Show(msg);
                return;
            }

            txtAddress.Text = vmessItem.address;
            txtPort.Text = vmessItem.port.ToString();
            cmbSecurity.Text = vmessItem.security;
            txtId.Text = vmessItem.id;
            txtRemarks.Text = vmessItem.remarks;
        }

        private void menuItemScanScreen_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            bgwScan.RunWorkerAsync();
        }

        #endregion

        private void bgwScan_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            string ret = Utils.ScanScreen();
            bgwScan.ReportProgress(0, ret);
        }

        private void bgwScan_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;

            string result = Convert.ToString(e.UserState);
            if (string.IsNullOrEmpty(result))
            {
                UI.Show("扫描完成,未发现有效二维码");
            }
            else
            {
                Utils.SetClipboardData(result);
                ImportConfig();
            }

        }

    }
}
