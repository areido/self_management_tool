/*memorize-area*/
/*
description:
    出社時・退勤時の体調等を記録

function:
    input:
        出社時、退勤時に押す。
        各種行動データと体調の変化を入力（保存先はCSV）
    output:
        推移の確認を行う際に押す。
        出力は日付順にソートしておく。（入力順に含める？）
    edit:
        ver3.0にて実装
        各打刻時データの修正を行いたい場合に押す。
    exit:
        プログラムを閉じる。

property:
    input:
        Menuからボタンで遷移
        入力項目は以下の通り
            ・就寝/起床時刻
            ・服薬時刻
            ・運動時間
            ver2.0:上記入力項目をカスタマイズ可能な状態にアップデート
            ver3.0:時刻入力の形式を変更
    output:
        Menuからボタンで遷移
        表示項目の指定を可能とする
    config:
        ver2.0で追加
        服薬関連のカスタマイズを行う。

other:
    ・グラフ化したい
        今のところは、Excelで開いてグラフ化が最適解

制作者:
    Kenya.Yamashita(mail: melt39miku@gmail.com)
*/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MainSystem{
    [STAThread]
    static void Main(){
        Application.Run(new Menu());
        Console.WriteLine("System Shutdown");
    }
}

class Menu : Form{
    Label label = new Label(){
        Text = "自己分析ツールメインメニューです。", 
        TabIndex = 0, 
        Location = new Point(75, 25), 
        AutoSize = true, 
    };

    Label tips = new Label(){
        Text = "ボタンにマウスポインターを乗せると、tipsを表示します。",
        TabIndex = 0,
        Location = new Point(75, 50),
        AutoSize = true,
    };

    Button input = new Button(){
        Text = "input", 
        TabIndex = 1, 
        Location = new Point(200, 75), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button edit = new Button(){
        Text = "edit", 
        TabIndex = 2, 
        Location = new Point(200, 100), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button reset = new Button(){
        Text = "reset", 
        TabIndex = 2, 
        Location = new Point(200, 125), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button output = new Button(){
        Text = "output", 
        TabIndex = 3, 
        Location = new Point(400, 75), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button config = new Button(){
        Text = "config", 
        TabIndex = 4, 
        Location = new Point(300, 175), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button exit = new Button(){
        Text = "exit", 
        TabIndex = 6, 
        Location = new Point(300, 225), 
        AutoSize = true, 
        Enabled = true, 
    };

    public Menu(){
        this.Width = 700;
        this.Height = 300;
        this.Controls.Add(label);
        this.Controls.Add(tips);
        this.Controls.Add(input);
        this.Controls.Add(reset);
        this.Controls.Add(edit);
        this.Controls.Add(output);
        this.Controls.Add(config);
        this.Controls.Add(exit);
        input.Click += new EventHandler(F_input);
        input.MouseHover += new EventHandler(H_input);
        reset.Click += new EventHandler(F_reset);
        reset.MouseHover += new EventHandler(H_reset);
        output.Click += new EventHandler(F_output);
        output.MouseHover += new EventHandler(H_output);
        edit.Click += new EventHandler(F_edit);
        edit.MouseHover += new EventHandler(H_edit);
        config.Click += new EventHandler(F_config);
        config.MouseHover += new EventHandler(H_config);
        exit.Click += new EventHandler(F_exit);
        exit.MouseHover += new EventHandler(H_exit);
    }

    private void F_input(object sender, EventArgs e){
        Input input = new Input();
        input.Show();
    }

    private void H_input(object sender, EventArgs e){
        this.tips.Text = "inputメニューを開きます。出退勤時の体調等を入力できます。";
    }

    private void F_reset(object sender, EventArgs e){
        StreamWriter init = new StreamWriter(@"自己管理シート.csv", false, Encoding.GetEncoding("UTF-8"));
        init.Write("");
        init.Close();
        MessageBox.Show("リセットしました", "result", MessageBoxButtons.OK);
    }

    private void H_reset(object sender, EventArgs e){
        this.tips.Text = "自己管理シート.csvに記録されたデータをリセット（削除）します。";
    }

    private void F_output(object sender, EventArgs e){
        Output output = new Output();
        output.Show();
    }

    private void H_output(object sender, EventArgs e){
        this.tips.Text = "outputメニューを開きます。自己管理シートに記録されたデータから、特定の項目だけを選んで抽出できます。";
    }

    private void F_edit(object sender, EventArgs e)
    {
        Edit edit = new Edit();
        edit.Show();
    }

    private void H_edit(object sender, EventArgs e)
    {
        this.tips.Text = "editメニューを開きます。自己管理シートに記録されたデータを選択し、内容を編集できます。";
    }
    
    private void F_config(object sender, EventArgs e){
        Config config = new Config();
        config.Show();
    }

    private void H_config(object sender, EventArgs e){
        this.tips.Text = "config画面を開きます。服薬状況についてカスタマイズが可能です。";
    }

    private void F_exit(object sender, EventArgs e){
        //終了コマンド
        this.Close();
    }

    private void H_exit(object sender, EventArgs e){
        this.tips.Text = "アプリケーションを終了します。";
    }
}

class Input : Form{

    Label label = new Label(){
        Text = "就寝時刻、起床時刻、服薬についてはhh:mm、運動時間は分単位の整数値で入力してください。", 
        TabIndex = 0, 
        Location = new Point(50, 25), 
        AutoSize = true, 
    };

    Label l_slept_start = new Label(){
        Text = "就寝",
        TabIndex = 99,
        Location = new Point(50,50),
        AutoSize = true,
    };

    DateTimePicker slept_start = new DateTimePicker(){
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "HH:mm",
        TabIndex = 1,
        Location = new Point(50, 75), 
        Width = 100,
        ShowUpDown = true,
    };

    Label l_slept_end = new Label(){
        Text = "起床",
        TabIndex = 99, 
        Location = new Point(150, 50), 
        AutoSize = true,  
    };

    DateTimePicker slept_end = new DateTimePicker(){
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "HH:mm",
        TabIndex = 2,
        Location = new Point(150, 75),
        Width = 100,
        ShowUpDown = true,
    };

    Label l_pill1 = new Label(){
        Text = "",
        TabIndex = 99,
        Location = new Point(250, 50),
        AutoSize = true,
    };

    DateTimePicker pill1 = new DateTimePicker(){
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "HH:mm",
        TabIndex = 3, 
        Location = new Point(250, 75), 
        Width = 100,
        ShowUpDown = true, 
    };

    Label l_pill2 = new Label(){
        Text = "",
        TabIndex = 99,
        Location = new Point(350, 50),
        AutoSize = true,
    };

    DateTimePicker pill2 = new DateTimePicker(){
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "HH:mm",
        TabIndex = 4, 
        Location = new Point(350, 75), 
        Width = 100,
        ShowUpDown = true,
    };

    Label l_pill3 = new Label(){
        Text = "",
        TabIndex = 99,
        Location = new Point(450, 50),
        AutoSize = true,
    };

    DateTimePicker pill3 = new DateTimePicker(){
        Format = DateTimePickerFormat.Custom,
        CustomFormat = "HH:mm",
        TabIndex = 5, 
        Location = new Point(450, 75),
        Width = 100,
        ShowUpDown = true,
    };

    Label l_training = new Label(){
        Text = "運動時間(分)",
        TabIndex = 99,
        Location = new Point(550, 50),
        AutoSize = true,
    };

    TextBox training = new TextBox(){
        Text = "",
        TabIndex = 6, 
        Location = new Point(550, 75), 
        AutoSize = true, 
        Enabled = true, 
    };

    CheckBox sleepy = new CheckBox(){
        Text = "眠気", 
        TabIndex = 7, 
        Location = new Point(150, 125), 
        AutoSize = true, 
        Checked = false, 
        Enabled = true, 
    };

    CheckBox panic = new CheckBox(){
        Text = "不安感・焦燥感", 
        TabIndex = 8, 
        Location = new Point(250, 125), 
        AutoSize = true, 
        Checked = false, 
        Enabled = true, 
    };

    Label l_feels = new Label(){
        Text = "気分",
        TabIndex = 99,
        Location = new Point(350, 100),
        AutoSize = true,
    };

    ComboBox feel = new ComboBox(){
        Items = { "悪い", "普通", "良い" },
        TabIndex = 9, 
        Location = new Point(350, 125), 
        AutoSize = true, 
    };

    Button submit = new Button(){
        Text = "submit", 
        TabIndex = 9, 
        Location = new Point(250, 150), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button exit = new Button(){
        Text = "exit", 
        TabIndex = 10, 
        Location = new Point(350, 150), 
        AutoSize = true, 
        Enabled = true, 
    };

    public Input(){
        this.Width = 700;
        this.Height = 250;
        this.Controls.Add(label);
        //就寝時刻フォーム
        this.Controls.Add(l_slept_start);
        this.Controls.Add(slept_start);

        //起床時刻フォーム
        this.Controls.Add(l_slept_end);
        this.Controls.Add(slept_end);

        //薬1服用時刻
        this.Controls.Add(l_pill1);
        this.Controls.Add(pill1);

        //薬2服用時刻
        this.Controls.Add(l_pill2);
        this.Controls.Add(pill2);

        //薬3服用時刻
        this.Controls.Add(l_pill3);
        this.Controls.Add(pill3);

        //運動時間
        this.Controls.Add(l_training);
        this.Controls.Add(training);

        //眠気チェックボックス
        this.Controls.Add(sleepy);

        //焦燥感チェックボックス
        this.Controls.Add(panic);

        //気分入力フォーム
        this.Controls.Add(l_feels);
        this.Controls.Add(feel);

        //入力確定ボタン
        this.Controls.Add(submit);
        submit.Click += new EventHandler(F_submit);

        //終了ボタン
        this.Controls.Add(exit);
        exit.Click += new EventHandler(F_exit);

        //出退勤切り替え
        DateTime dt = DateTime.Now;
        StreamReader stream01 = new StreamReader(@"自己管理シート.csv", Encoding.GetEncoding("UTF-8"));
        List<string> recent = new List<string>();
        while(!stream01.EndOfStream){
            recent.Add(stream01.ReadLine());
        }
        string yymmdd = dt.Year + "/" + dt.Month + "/" + dt.Day;
        bool sw = false;
        foreach(var val in recent){
            if(val.IndexOf(yymmdd) != -1){
                sw = true;
            }
        }
        if(sw){
            this.Text = "退勤時記録";
        }
        else{
            this.Text = "出勤時記録";
        }
        stream01.Close();

        //設定ファイル読み込み
        StreamReader stream02 = new StreamReader(@"config.csv", Encoding.GetEncoding("UTF-8"));
        List<string> config = new List<string>();
        while(!stream02.EndOfStream){
            config.Add(stream02.ReadLine());
        }
        if(config[0] == "TRUE"){
            pill1.Enabled = true;
            l_pill1.Text = config[1];
        }
        else{
            pill1.Enabled = false;
        }
        if(config[2] == "TRUE"){
            pill2.Enabled = true;
            l_pill2.Text = config[3];
        }
        else{
            pill2.Enabled = false;
        }
        if(config[4] == "TRUE"){
            pill3.Enabled = true;
            l_pill3.Text = config[5];
        }
        else{
            pill3.Enabled = false;
        }
        stream02.Close();
    }

    private void clear(object sender, EventArgs e){
        //クリック時に入力フォームリセット
        ((TextBox)sender).Text = "";
    }

    private void F_submit(object sender, EventArgs e){
        //睡眠時間の算出
        string total = (slept_end.Value - slept_start.Value).ToString();
        //CSVに書き出し
        List<string> data = new List<string>();
        DateTime dt = DateTime.Now;
        data.Add(slept_start.Text);
        data.Add(slept_end.Text);
        data.Add(total);
        if(pill1.Enabled){
            data.Add(pill1.Text);
        }else{
            data.Add("");
        }
        if(pill2.Enabled){
            data.Add(pill2.Text);
        }else{
            data.Add("");
        }
        if(pill3.Enabled){
            data.Add(pill3.Text);
        }else{
            data.Add("");
        }
        data.Add(training.Text);
        if(sleepy.Checked == true){
            data.Add("1");
        }
        else{
            data.Add("0");
        }
        if(panic.Checked == true){
            data.Add("1");
        }
        else{
            data.Add("0");
        }
        data.Add(feel.Text);

        StreamReader recent = new StreamReader(@"自己管理シート.csv", Encoding.GetEncoding("UTF-8"));
        List<string> lines = new List<string>();
        while(!recent.EndOfStream){
            lines.Add(recent.ReadLine());
        }
        string output = "";
        string yymmdd = dt.Year + "/" + dt.Month + "/" + dt.Day;
        output = yymmdd + "," + dt.ToShortTimeString() + ",";
        bool sw = false;
        foreach(var val in lines){
            if(val.IndexOf(yymmdd) != -1){
                sw=true;
            }
        }
        if(sw==true){
            output = output + "退勤,";
        }
        else{
            output = output + "出勤,";
        }
        foreach(var val in data){
            output = output + (string)val + ",";
        }
        output.Remove(output.Length-1, 1);
        recent.Close();
        StreamWriter s_writer = new StreamWriter(@"自己管理シート.csv", true, Encoding.GetEncoding("UTF-8"));        
        Console.WriteLine("debug_output=" + output);
        s_writer.Write(output + "\n");
        s_writer.Close();
        MessageBox.Show("入力完了しました", "result", MessageBoxButtons.OK);
    }

    private void F_exit(object sender, EventArgs e){
        //終了コマンド
        this.Close();
    }
}

class Output : Form{
    protected bool flag = true;

    Label label = new Label(){
        Text = "出力したい項目を選択してください。", 
        TabIndex = 0, 
        Location = new Point(50, 0), 
        AutoSize = true, 
    };

    Label debug = new Label(){
        Text = "Notice: Constracting", 
        TabIndex = 1, 
        Location = new Point(50, 50), 
        AutoSize = true, 
    };

    CheckBox yymmdd = new CheckBox(){
        Text = "記録日時", 
        TabIndex = 0, 
        Location = new Point(0, 0), 
        AutoSize = true, 
        Checked = true, 
        Enabled = false, 
        Visible = false, 
    };

    CheckBox hhmm = new CheckBox(){
        Text = "記録時刻", 
        TabIndex = 0, 
        Location = new Point(0, 0), 
        AutoSize = true, 
        Checked = true, 
        Enabled = false, 
        Visible = false, 
    };

    CheckBox stat = new CheckBox(){
        Text = "出退勤", 
        TabIndex = 0, 
        Location = new Point(0, 0), 
        AutoSize = true, 
        Checked = true, 
        Enabled = false, 
        Visible = false, 
    };

    CheckBox slept_start = new CheckBox(){
        Text = "就寝時刻", 
        TabIndex = 2, 
        Location = new Point(50, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox slept_end = new CheckBox(){
        Text = "起床時刻", 
        TabIndex = 3, 
        Location = new Point(150, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox slept_total = new CheckBox(){
        Text = "睡眠時間", 
        TabIndex = 4, 
        Location = new Point(250, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox pill1 = new CheckBox(){
        Text = "", 
        TabIndex = 5, 
        Location = new Point(350, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox pill2 = new CheckBox(){
        Text = "", 
        TabIndex = 6, 
        Location = new Point(450, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox pill3 = new CheckBox(){
        Text = "", 
        TabIndex = 7, 
        Location = new Point(550, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox training = new CheckBox(){
        Text = "運動時間", 
        TabIndex = 8, 
        Location = new Point(650, 50), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox sleepy = new CheckBox(){
        Text = "眠気", 
        TabIndex = 9, 
        Location = new Point(150, 75), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox panic = new CheckBox(){
        Text = "焦燥感", 
        TabIndex = 10, 
        Location = new Point(250, 75), 
        AutoSize = true, 
        Checked = true, 
    };

    CheckBox feel = new CheckBox(){
        Text = "気分", 
        TabIndex = 11, 
        Location = new Point(350, 75), 
        AutoSize = true, 
        Checked = true, 
    };    

    Button submit = new Button(){
        Text = "submit", 
        TabIndex = 12, 
        Location = new Point(250, 100), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button exit = new Button(){
        Text = "exit", 
        TabIndex = 13, 
        Location = new Point(350, 100), 
        AutoSize = true, 
        Enabled = true, 
    };

    public Output(){
        this.Width = 800;
        this.Height = 200;
        this.Controls.Add(label);
        //this.Controls.Add(debug);

        //就寝時刻フォーム
        this.Controls.Add(slept_start);
        //slept_start.Click += new EventHandler(clear);

        //起床時刻フォーム
        this.Controls.Add(slept_end);
        //slept_end.Click += new EventHandler(clear);

        //睡眠時間フォーム
        this.Controls.Add(slept_total);

        //薬1服用時刻
        this.Controls.Add(pill1);
        //pill1.Click += new EventHandler(clear);

        //薬2服用時刻
        this.Controls.Add(pill2);
        //pill2.Click += new EventHandler(clear);

        //薬3服用時刻
        this.Controls.Add(pill3);
        //pill3.Click += new EventHandler(clear);

        //運動時間
        this.Controls.Add(training);
        //training.Click += new EventHandler(clear);

        //眠気
        this.Controls.Add(sleepy);

        //焦燥感
        this.Controls.Add(panic);

        //気分
        this.Controls.Add(feel);

        //入力確定ボタン
        this.Controls.Add(submit);
        submit.Click += new EventHandler(F_submit);

        //終了ボタン
        this.Controls.Add(exit);
        exit.Click += new EventHandler(F_exit);

        //設定ファイル読み込み
        StreamReader stream = new StreamReader(@"config.csv", Encoding.GetEncoding("UTF-8"));
        List<string> config = new List<string>();
        while(!stream.EndOfStream){
            config.Add(stream.ReadLine());
        }
        if(config[0] == "TRUE"){
            pill1.Enabled = true;
            pill1.Checked = true;
            pill1.Text = config[1];
        }
        else{
            pill1.Enabled = false;
            pill1.Checked = false;
            pill1.Text = "無効";
        }
        if(config[2] == "TRUE"){
            pill2.Enabled = true;
            pill2.Checked = true;
            pill2.Text = config[3];
        }
        else{
            pill2.Enabled = false;
            pill2.Checked = false;
            pill2.Text = "無効";
        }
        if(config[4] == "TRUE"){
            pill3.Enabled = true;
            pill3.Checked = true;
            pill3.Text = config[5];
        }
        else{
            pill3.Enabled = false;
            pill3.Checked = false;
            pill3.Text = "無効";
        }
        stream.Close();
    }

    private void F_submit(object sender, EventArgs e){
        //debug.Text = "Notice: What are you doing!?!? are you serious!?!?!?!?";

        //work in progress
        //file_name
        List<CheckBox> checklist = new List<CheckBox>();
        
        //チェックボックス参照
        //チェックが入っている項目のみ抽出し、テーブル化
        //レコード単位で行うため、項目を配列として格納するのが理想？
        checklist.Add(yymmdd);
        checklist.Add(hhmm);
        checklist.Add(stat);
        checklist.Add(slept_start);
        checklist.Add(slept_end);
        checklist.Add(slept_total);
        if(pill1.Enabled){
            checklist.Add(pill1);
        }
        if(pill2.Enabled){
            checklist.Add(pill2);
        }
        if(pill3.Enabled){
            checklist.Add(pill3);
        }
        checklist.Add(training);
        checklist.Add(sleepy);
        checklist.Add(panic);
        checklist.Add(feel);

        //proc_start
        System.Text.Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
        StreamReader stream = new StreamReader(@"自己管理シート.csv", Encoding.GetEncoding("UTF-8"));
        StreamWriter s_writer = new StreamWriter(@"analyzed.csv", false, Encoding.GetEncoding("UTF-8"));
        List<string> lines = new List<string>();
        while(!stream.EndOfStream){
            lines.Add(stream.ReadLine());
        }
        string output = "";
        foreach(var val2 in checklist){
            if(val2.Checked == true){
                output = output + val2.Text + ",";
            }
        }
        output.Remove(output.Length-1, 1);
        s_writer.WriteLine(output);
        foreach(var val in lines){
            string[] current = val.Split(',');
            output = "";
            int index = 0;
            foreach(var val2 in checklist){
                if(val2.Checked==true){
                    output = output + current[index] + ",";
                }
                index++;
            }
            output.Remove(output.Length-1, 1);
            s_writer.WriteLine(output);
        }
        s_writer.Close();

        MessageBox.Show("出力しました\nanalyzed.csvをご確認ください", "result", MessageBoxButtons.OK);

    }

    private void F_exit(object sender, EventArgs e){
        //終了コマンド
        flag = false;
        this.Close();
    }
}

//データ編集画面
class Edit : Form{
    Label label = new Label(){
        Text = "データ編集画面です。\n編集したいデータの日付と出退勤ステータスを選択してください。",
        TabIndex = 0,
        Location = new Point(50, 25),
        AutoSize = true,
    };

    ComboBox d_list = new ComboBox()
    {
        TabIndex = 1,
        Location = new Point(50, 60),
        AutoSize = true,
    };

    Button edit = new Button()
    {
        Text = "edit",
        TabIndex = 2,
        Location = new Point(50, 85),
        AutoSize = true,
    };

    Label l_slept_start = new Label()
    {
        Text = "就寝",
        Location = new Point(50, 150),
        AutoSize = true,
        Visible = false,
    };

    DateTimePicker slept_start = new DateTimePicker()
    {
        Format = DateTimePickerFormat.Custom,
        ShowUpDown = true,
        CustomFormat = "HH:mm",
        Width = 100,
        Location = new Point(50, 175),
        Visible = false,
    };

    Label l_slept_end = new Label()
    {
        Text = "起床",
        Location = new Point(150, 150),
        AutoSize = true,
        Visible = false,
    };

    DateTimePicker slept_end = new DateTimePicker()
    {
        Format = DateTimePickerFormat.Custom,
        ShowUpDown = true,
        CustomFormat = "HH:mm",
        Width = 100,
        Location = new Point(150, 175),
        Visible = false,
    };

    Label l_pill1 = new Label()
    {
        Text = "",
        Location = new Point(250, 150),
        AutoSize = true,
        Visible = false,
    };

    DateTimePicker pill1 = new DateTimePicker()
    {
        Format = DateTimePickerFormat.Custom,
        ShowUpDown = true,
        CustomFormat = "HH:mm",
        Width = 100,
        Location = new Point(250, 175),
        Visible = false,
    };

    Label l_pill2 = new Label()
    {
        Text = "",
        Location = new Point(350, 150),
        AutoSize = true,
        Visible = false,
    };

    DateTimePicker pill2 = new DateTimePicker()
    {
        Format = DateTimePickerFormat.Custom,
        ShowUpDown = true,
        CustomFormat = "HH:mm",
        Width = 100,
        Location = new Point(350, 175),
        Visible = false,
    };

    Label l_pill3 = new Label()
    {
        Text = "",
        Location = new Point(450, 150),
        AutoSize = true,
        Visible = false,
    };

    DateTimePicker pill3 = new DateTimePicker()
    {
        Format = DateTimePickerFormat.Custom,
        ShowUpDown = true,
        CustomFormat = "HH:mm",
        Width = 100,
        Location = new Point(450, 175),
        Visible = false,
    };

    Label l_training = new Label()
    {
        Text = "運動時間",
        Location = new Point(550, 150),
        AutoSize = true,
        Visible = false,
    };

    ComboBox training = new ComboBox()
    {
        Location = new Point(550, 175),
        Width = 100,
        Visible = false,
    };

    CheckBox sleepy = new CheckBox()
    {
        Text = "眠気",
        Location = new Point(50, 225),
        AutoSize = true,
        Visible = false,
    };

    CheckBox panic = new CheckBox()
    {
        Text = "不安感・焦燥感",
        Location = new Point(150, 225),
        AutoSize = true,
        Visible = false,
    };

    Label l_feels = new Label()
    {
        Text = "気分",
        Location = new Point(250, 200),
        AutoSize = true,
        Visible = false,
    };

    ComboBox feels = new ComboBox()
    {
        Items = { "悪い", "普通", "良い" },
        Location = new Point(250, 225),
        Width = 100,
        Visible = false,
    };

    Label l_tstump = new Label(){
        Text = "タイムスタンプ",
        Location = new Point(350, 200),
        AutoSize = true,
        Visible = false,
    };

    DateTimePicker tstump = new DateTimePicker(){
        Format = DateTimePickerFormat.Custom,
        ShowUpDown = true,
        CustomFormat = "HH:mm",
        Width = 100,
        Location = new Point(350, 225),
        Visible = false,
    };

    Button submit = new Button()
    {
        Text = "submit",
        TabIndex = 3,
        Location = new Point(325, 300),
        Visible = false,
        Enabled = false
    };

    Button exit = new Button()
    {
        Text = "exit",
        TabIndex = 4,
        Location = new Point(325, 350),
        AutoSize = true,
    };

    public Edit()
    {
        this.Width = 700;
        this.Height = 400;
        this.Controls.Add(label);
        this.Controls.Add(d_list);
        this.Controls.Add(edit);
        this.Controls.Add(l_slept_start);
        this.Controls.Add(slept_start);
        this.Controls.Add(l_slept_end);
        this.Controls.Add(slept_end);
        this.Controls.Add(l_pill1);
        this.Controls.Add(pill1);
        this.Controls.Add(l_pill2);
        this.Controls.Add(pill2);
        this.Controls.Add(l_pill3);
        this.Controls.Add(pill3);
        this.Controls.Add(l_training);
        this.Controls.Add(training);
        this.Controls.Add(sleepy);
        this.Controls.Add(panic);
        this.Controls.Add(l_feels);
        this.Controls.Add(feels);
        this.Controls.Add(l_tstump);
        this.Controls.Add(tstump);
        this.Controls.Add(submit);
        this.Controls.Add(exit);

        StreamReader current = new StreamReader(@"自己管理シート.csv", Encoding.GetEncoding("UTF-8"));
        List<string> lines = new List<string>();
        while (!current.EndOfStream)
        {
            lines.Add(current.ReadLine());
        }
        int seq = 0;
        foreach(var val in lines)
        {
            string[] sp = val.Split(',');
            string data = seq.ToString() + ',' + sp[0] + ',' + sp[2];
            d_list.Items.Add(data);
            seq++;
        }
        d_list.SelectedIndex = 0;

        for(int i = 0; i <= 60; i++)
        {
            training.Items.Add(i);
        }
        training.SelectedIndex = 0;

        edit.Click += new EventHandler(F_edit);
        submit.Click += new EventHandler(F_submit);
        exit.Click += new EventHandler(F_exit);
        current.Close();
    }

    private void F_edit(object sender, EventArgs e)
    {
        int num = d_list.SelectedIndex;
        StreamReader file = new StreamReader(@"自己管理シート.csv", Encoding.GetEncoding("utf-8"));
        List<string> lines = new List<string>();
        while (!file.EndOfStream)
        {
            lines.Add(file.ReadLine());
        }
        l_slept_start.Visible = true;
        l_slept_end.Visible = true;
        l_pill1.Visible = true;
        l_pill2.Visible = true;
        l_pill3.Visible = true;
        l_training.Visible = true;
        l_feels.Visible = true;
        l_tstump.Visible = true;
        slept_start.Visible = true;
        slept_end.Visible = true;
        pill1.Visible = true;
        pill2.Visible = true;
        pill3.Visible = true;
        training.Visible = true;
        sleepy.Visible = true;
        panic.Visible = true;
        feels.Visible = true;
        tstump.Visible = true;
        submit.Visible = true;

        sleepy.Enabled = true;
        panic.Enabled = true;
        feels.Enabled = true;
        submit.Enabled = true;

        string[] target_data = lines[num].Split(',');
        tstump.Value = DateTime.Parse(target_data[1]);
        slept_start.Value = DateTime.Parse(target_data[3]);
        slept_end.Value = DateTime.Parse(target_data[4]);
        if(target_data[6] != ""){
            pill1.Value = DateTime.Parse(target_data[6]);
        }else{
            pill1.Value = DateTime.Now;
            pill1.Enabled = false;
        }
        if(target_data[7] != ""){
            pill2.Value = DateTime.Parse(target_data[7]);
        }else{
            pill2.Value = DateTime.Now;
            pill2.Enabled = false;
        }
        if(target_data[8] != ""){
            pill3.Value = DateTime.Parse(target_data[8]);
        }else{
            pill3.Value = DateTime.Now;
            pill3.Enabled = false;
        }
        training.ValueMember = target_data[9];
        if (target_data[10] == "1")
        {
            sleepy.Checked = true;
        }
        else
        {
            sleepy.Checked = false;
        }
        if (target_data[11] == "1")
        {
            panic.Checked = true;
        }
        else
        {
            panic.Checked = false;
        }
        feels.ValueMember = target_data[12];
        file.Close();
    }

    private void F_submit(object sender, EventArgs e)
    {
        int num = d_list.SelectedIndex;
        StreamReader file = new StreamReader(@"自己管理シート.csv", Encoding.GetEncoding("utf-8"));
        List<string> lines = new List<string>();
        while(!file.EndOfStream)
        {
            lines.Add(file.ReadLine());
        }

        int seq = 0;
        string output = "";
        foreach(var val in lines)
        {
            if(num == seq)
            {
                string[] data = val.Split(',');
                data[1] = tstump.Text;
                data[3] = slept_start.Text;
                data[4] = slept_end.Text;
                data[5] = (slept_end.Value - slept_start.Value).ToString();
                if(pill1.Enabled){
                    data[6] = pill1.Text;
                }else{
                    data[6] = "";
                }
                if(pill2.Enabled){
                    data[7] = pill2.Text;
                }else{
                    data[7] = "";
                }
                if(pill3.Enabled){
                    data[8] = pill3.Text;
                }else{
                    data[8] = "";
                }
                data[9] = training.Text;
                if(sleepy.Checked)
                {
                    data[10] = "1";
                }
                else
                {
                    data[10] = "0";
                }
                if (panic.Checked)
                {
                    data[11] = "1";
                }
                else
                {
                    data[11] = "0";
                }
                    data[12] = feels.Text;
                foreach(var val2 in data)
                {
                    output = output + val2.ToString() + ",";
                }
                output.Remove(output.Length - 1, 1);
                output = output + "\n";
            }
            else
            {
                output = output + val + "\n";
            }
            seq++;
        }
        file.Close();
        StreamWriter o_file = new StreamWriter(@"自己管理シート.csv", false, Encoding.GetEncoding("utf-8"));
        o_file.Write(output);
        o_file.Close();
        MessageBox.Show("変更が完了しました。", "result", MessageBoxButtons.OK);
    }

    private void F_exit(object sender, EventArgs e)
    {
        this.Close();
    }
}

//服薬情報設定画面
class Config : Form{
    Label label = new Label(){
        Text = "設定画面です。\n服薬状況を3種類まで設定できます。", 
        TabIndex = 0, 
        Location = new Point(50, 25), 
        AutoSize = true, 
    };

    CheckBox pill1 = new CheckBox(){
        Text = "服薬1", 
        TabIndex = 1, 
        Location = new Point(50, 50), 
        AutoSize = true, 
    };

    TextBox pill1t = new TextBox(){
        Text = "", 
        TabIndex = 2, 
        Location = new Point(50, 75), 
        AutoSize = true, 
        Enabled = false, 
    };

    CheckBox pill2 = new CheckBox(){
        Text = "服薬2", 
        TabIndex = 3, 
        Location = new Point(150, 50), 
        AutoSize = true, 
        Enabled = false, 
    };

    TextBox pill2t = new TextBox(){
        Text = "", 
        TabIndex = 4, 
        Location = new Point(150, 75), 
        AutoSize = true, 
        Enabled = false, 
    };

    CheckBox pill3 = new CheckBox(){
        Text = "服薬3", 
        TabIndex = 5, 
        Location = new Point(250, 50), 
        AutoSize = true, 
        Enabled = false, 
    };

    TextBox pill3t = new TextBox(){
        Text = "", 
        TabIndex = 6, 
        Location = new Point(250, 75), 
        AutoSize = true, 
        Enabled = false, 
    };

    Button submit = new Button(){
        Text = "submit", 
        TabIndex = 7, 
        Location = new Point(300, 175), 
        AutoSize = true, 
        Enabled = true, 
    };

    Button exit = new Button(){
        Text = "exit", 
        TabIndex = 8, 
        Location = new Point(300, 200), 
        AutoSize = true, 
        Enabled = true, 
    };

    public Config(){
        this.Width = 700;
        this.Height = 300;
        this.Controls.Add(label);
        this.Controls.Add(exit);
        this.Controls.Add(submit);
        this.Controls.Add(pill1);
        this.Controls.Add(pill1t);
        this.Controls.Add(pill2);
        this.Controls.Add(pill2t);
        this.Controls.Add(pill3);
        this.Controls.Add(pill3t);
        pill1.Click += new EventHandler(F_pill1_switcher);
        pill2.Click += new EventHandler(F_pill2_switcher);
        pill3.Click += new EventHandler(F_pill3_switcher);
        exit.Click += new EventHandler(F_exit);
        submit.Click += new EventHandler(F_submit);
        StreamReader recent = new StreamReader(@"config.csv", Encoding.GetEncoding("UTF-8"));
        List<string> lines = new List<string>();
        while(!recent.EndOfStream){
            lines.Add(recent.ReadLine());
        }
        //recentから直近の設定を読み込み
        if(lines[0] == "TRUE"){
            pill1.Checked = true;
            pill1t.Enabled = true;
            pill1t.Text = lines[1];
            pill2.Enabled = true;
        }
        else{
            pill1.Checked = false;
            pill1t.Enabled = false;
        }
        if(lines[2] == "TRUE"){
            pill2.Checked = true;
            pill2t.Enabled = true;
            pill2t.Text = lines[3];
            pill3.Enabled = true;
        }
        else{
            pill2.Checked = false;
            pill2t.Enabled = false;
        }
        if(lines[4] == "TRUE"){
            pill3.Checked = true;
            pill3t.Enabled = true;
            pill3t.Text = lines[5];
        }
        else{
            pill3.Checked = false;
            pill3t.Enabled = false;
        }
        recent.Close();

    }

    private void F_pill1_switcher(object sender, EventArgs e){
        if(pill1.Checked){
            pill1t.Enabled = true;
            pill2.Enabled = true;
        }
        else{
            pill1t.Enabled = false;
            pill1t.Text = "";
            pill2.Checked = false;
            pill2.Enabled = false;
            pill2t.Enabled = false;
            pill2t.Text = "";
            pill3.Checked = false;
            pill3.Enabled = false;
            pill3t.Enabled = false;
            pill3t.Text = "";
        }
    }

    private void F_pill2_switcher(object sender, EventArgs e){
        if(pill2.Checked){
            pill2t.Enabled = true;
            pill3.Enabled = true;
        }
        else{
            pill2t.Enabled = false;
            pill2t.Text = "";
            pill3.Checked = false;
            pill3.Enabled = false;
            pill3t.Enabled = false;
            pill3t.Text = "";
        }
    }

    private void F_pill3_switcher(object sender, EventArgs e){
        if(pill3.Checked){
            pill3t.Enabled = true;
        }
        else{
            pill3t.Enabled = false;
            pill3t.Text = "";
        }
    }

    private void F_submit(object sender, EventArgs e){
        //Config出力コマンド
        List<string> data = new List<string>();
        if(pill1.Checked && pill1t.Text != ""){
            data.Add("TRUE");
        }
        else{
            data.Add("FALSE");
        }
        data.Add(pill1t.Text);

        if(pill2.Checked && pill2t.Text != ""){
            data.Add("TRUE");
        }
        else{
            data.Add("FALSE");
        }
        data.Add(pill2t.Text);

        if(pill3.Checked && pill3t.Text != ""){
            data.Add("TRUE");
        }
        else{
            data.Add("FALSE");
        }
        data.Add(pill3t.Text);
        string output = "";
        foreach(var val in data){
            output = output + val + "\n";
        }
        StreamWriter config = new StreamWriter(@"config.csv", false, Encoding.GetEncoding("UTF-8"));
        config.Write(output);
        config.Close();
    }

    private void F_exit(object sender, EventArgs e){
        //終了コマンド
        this.Close();
    }
}