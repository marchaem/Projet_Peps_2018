<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Web._Default" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<%@ Register Assembly="BewiseFlashControl" Namespace="Bewise.Web.UI.WebControls" TagPrefix="Bewise" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>ASP.NET</h1>
        <p class="lead">ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS, and JavaScript.</p>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

                    <div style="float: left; margin-left: 30px;">
                    <select id="ddl_one">
                        <option value="1">Moto G</option>
                        <option value="2">IPhone 5</option>
                        <option value="3">Samsung S5</option>
                    </select>
                    <select id="ddl_two">
                        <option value="1">Moto G</option>
                        <option value="2">IPhone 5</option>
                        <option value="3">Samsung S5</option>
                    </select>
                    <select id="ddlYear">
                        <option>2013</option>
                        <option>2014</option>
                        <option>2015</option>
                    </select>
                    <input id="btn_line_chart" type="button" value="Show" /></div>
                </div>
                <br />
                <div>
                <canvas id="myChart"> </canvas>
                </div>


    <div class="row">
        <script>
            let myChart = document.getElementById('myChart').getContext('2d');
            let dataChart = new Chart(myChart, {
                type: 'line',
                data: {
                     
                },
            })
        </script>
        <div class="col-md-4">
            <h2>Getting started</h2>
            <p>
                
                <asp:TextBox ID="from_date" runat="server" placeholder="From" type="date" ></asp:TextBox>
                <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
                <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click" />
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301948">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Get more libraries</h2>
           
            <p>
                <asp:TextBox ID="from_date2" runat="server" placeholder="From" type="date" ></asp:TextBox>
                <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301949">Learn more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Web Hosting</h2>
            <p>
                You can easily find a web hosting company that offers the right mix of features and price for your applications.
            </p>
            <p>
                <a class="btn btn-default" href="http://go.microsoft.com/fwlink/?LinkId=301950">Learn more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
