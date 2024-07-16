<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="ClaimPortalUserCreation.Index" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Index</title>
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0-beta3/css/all.min.css" rel="stylesheet" />

    <style>
        .toast-custom {
            position: fixed;
            top: 10px;
            right: 20px;
            width: 300px;
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            padding: 15px;
        }

        .toast-success {
            border-left: 5px solid #28a745; /* Light green */
        }

        .toast-danger {
            border-left: 5px solid #dc3545; /* Red */
        }

        .form-control {
            text-align: center;
        }

        .col-lg-2-4 {
            flex: 0 0 20%;
            max-width: 20%;
        }

        @media (max-width: 768px) {
            .col-12 {
                text-align: center;
            }

            #btnOpenModal {
                width: 100%;
            }

            .grid-wrapper {
                overflow-x: auto;
                -webkit-overflow-scrolling: touch;
            }

                .grid-wrapper table {
                    width: 100%;
                    display: block;
                }

                    .grid-wrapper table thead,
                    .grid-wrapper table tbody {
                        display: table;
                        width: 100%;
                    }
        }

        .curved-container {
            border: 2px solid #ccc;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 20px 20px 8px rgba(0,0,0,0.1);
        }

        .grid-wrapper {
            max-height: 300px;
            width: 100%;
            overflow: auto;
        }
    </style>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js"></script>
    <script>
        function showToast(message, styleClass) {
            var toast = $('<div class="toast-custom ' + styleClass + '">' + message + '</div>').appendTo('#toastContainer');

            // Show the toast
            toast.fadeIn();

            // Move existing toasts down
            $('.toast-custom').not(toast).each(function () {
                $(this).animate({ top: "+=" + (toast.outerHeight() + 10) }, 'fast');
            });

            // Hide the toast after 3 seconds
            setTimeout(function () {
                toast.fadeOut(function () {
                    // Remove the toast from DOM after fadeOut
                    $(this).remove();

                    // Move remaining toasts up
                    $('.toast-custom').each(function (index) {
                        $(this).animate({ top: "-=" + (toast.outerHeight() + 10) }, 'fast');
                    });
                });
            }, 3000);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <%-- CreateDiv --%>
            <div class="body-content" id="CreateDiv" runat="server" style="display: block;">
                <h2 style="text-align: center; margin-top: 20px;">Claim Portal User Creation</h2>
                <div class="headtag">
                    <asp:Label ID="lblUserName" runat="server" Style="color: black; float: right; margin-top: -20px; margin-right: 20px"></asp:Label>
                </div>
                <hr />

                <div class="container-fluid d-flex justify-content-center align-items-center" style="">
                    <div class="curved-container ">
                        <div class="row" style="margin-bottom: -20px;">
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:DropDownList ID="StateDrp" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="StateDrp_SelectedIndexChanged">
                                    <asp:ListItem Text="State" Value="StateName"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:DropDownList ID="AreaDrp" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="AreaDrp_SelectedIndexChanged">
                                    <asp:ListItem Text="Area" Value="AreaName"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:DropDownList ID="ZoneDrp" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ZoneDrp_SelectedIndexChanged">
                                    <asp:ListItem Text="Zone" Value="DistrictName"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:DropDownList ID="UserTypeDrp" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="UserTypeDrp_SelectedIndexChanged">
                                    <asp:ListItem Text="UserType" Value="UserType"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="UserId" runat="server" AutoPostBack="true" CssClass="form-control" placeholder="User Id" OnTextChanged="UserId_TextChanged"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="UserName" runat="server" AutoPostBack="true" CssClass="form-control" placeholder="User Name" OnTextChanged="UserName_TextChanged"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="UserEmail" type="email" runat="server" CssClass="form-control" placeholder="User Email"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="UserMob" type="tel" runat="server" CssClass="form-control" placeholder="User Moile No."></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3 position-relative">
                                <asp:TextBox ID="UserPass" type="password" runat="server" CssClass="form-control" placeholder="User Password"></asp:TextBox>
                                <i class="fa fa-eye position-absolute" id="togglePassword" style="cursor: pointer; top: 50%; right: 25px; transform: translateY(-50%); opacity: 75%"></i>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:Button ID="Button2" runat="server" Text="Create" CssClass="btn btn-outline-primary form-control" OnClick="BtnSubmit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <%-- confimDivDiv --%>
            <div class="container body-content" id="ConfirmDiv" runat="server" style="display: none;">
                <br />
                <br />
                <div class="container-fluid d-flex justify-content-center align-items-center">
                    <div class="curved-container ">
                        <div class="row" style="margin-bottom: -20px;">
                            <div class=" col-md-6 col-sm-12 mb-3">
                                Data Already Exists. Do you want to Modify?
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12 mb-3">
                                <asp:Button ID="ConfirmYes" runat="server" Text="Yes" CssClass="btn btn-outline-success form-control" OnClick="Confirm_Click" />
                            </div>
                            <div class="col-lg-3 col-md-3 col-sm-12 mb-3">
                                <asp:Button ID="ConfirmNo" runat="server" Text="No" CssClass="btn btn-outline-danger form-control" OnClick="Confirm_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <%-- EditDiv --%>
            <div class="container body-content" id="EditDiv" runat="server" style="display: none;">
                <h2 style="text-align: center; margin-top: 20px;">Claim Portal User Modification</h2>
                <div class="headtag">
                    <asp:Label ID="lblUserName1" runat="server" Style="color: black; float: right; margin-top: -20px; margin-right: 20px"></asp:Label>
                </div>
                <hr />

                <div class="container-fluid d-flex justify-content-center align-items-center">
                    <div class="curved-container ">
                        <div class="row" style="margin-bottom: -20px;">
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="StateMod" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="AreaMod" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="ZoneMod" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="UserTypeMod" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="UserIdMod" runat="server" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="NameMod" runat="server" CssClass="form-control" placeholder="User Name"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="MailMod" type="email" runat="server" CssClass="form-control" placeholder="User Email"></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:TextBox ID="MobMod" type="tel" runat="server" CssClass="form-control" placeholder="User Moile No."></asp:TextBox>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3 position-relative">
                                <asp:TextBox ID="PassMod" type="password" runat="server" CssClass="form-control" placeholder="User Password"></asp:TextBox>
                                <i class="fa fa-eye position-absolute" id="togglePassword1" style="cursor: pointer; top: 50%; right: 25px; transform: translateY(-50%); opacity: 75%"></i>
                            </div>
                            <div class="col-lg-2-4 col-md-6 col-sm-12 mb-3">
                                <asp:Button ID="BtnModify" runat="server" Text="Modify" CssClass="btn btn-outline-success form-control" OnClick="BtnModify_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <%-- GridView --%>
            <br />
            <br />
            <div class="container-fluid d-flex justify-content-center align-items-center" style="">
                <div class="row">
                    <div class="col-12">
                        <div class="grid-wrapper">
                            <asp:GridView ID="CPGridView" runat="server" AutoGenerateColumns="false" CssClass="table table-bordered"
                                Style="margin-bottom: 0px; text-align: center;" OnRowDataBound="CPGridView_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="UserId" HeaderText="User Id" />
                                    <asp:BoundField DataField="StateName" HeaderText="State Name" />
                                    <asp:BoundField DataField="AreaName" HeaderText="Area Name" />
                                    <asp:BoundField DataField="ZoneName" HeaderText="Zone Name" />
                                    <asp:BoundField DataField="UserName" HeaderText="User Name" />
                                    <asp:BoundField DataField="UserEmail" HeaderText="User Email" />
                                    <asp:BoundField DataField="UserMobileNo" HeaderText="User Mobile No." />
                                    <%--<asp:BoundField DataField="Password" HeaderText="Password" />--%>

                                    <asp:TemplateField HeaderText="Password">
                                        <ItemTemplate>
                                            <asp:Label ID="PasswordLabel" runat="server" Text='<%# MaskPassword(Eval("Password").ToString()) %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Create/Modify">
                                        <ItemTemplate>
                                            <asp:PlaceHolder ID="ButtonPlaceHolder" runat="server">
                                                <asp:Button ID="EditBtn" runat="server" Text="Modify" CssClass="btn btn-outline-success form-control" Visible='<%# Eval("Selected").ToString() == "1" %>' OnClick="EditBtn_Click" />
                                                <asp:Button ID="CreateBtn" runat="server" Text="Create" CssClass="btn btn-outline-primary form-control" Visible='<%# Eval("Selected").ToString() == "0" %>' OnClick="CreateBtn_Click" />
                                                <asp:TextBox ID="ExitsBox" runat="server" Text="Exists" CssClass="form-control" Visible='<%# Eval("Selected").ToString() == "1" %>' ReadOnly="true" />
                                                <asp:TextBox ID="CreateBox" runat="server" Text="Need to Create" CssClass="form-control" Visible='<%# Eval("Selected").ToString() == "0" %>' ReadOnly="true" />
                                                <%--<asp:TextBox ID="UserId" runat="server" AutoPostBack="true" CssClass="form-control" placeholder="Need to Create" ReadOnly="true"></asp:TextBox>--%>
                                            </asp:PlaceHolder>
                                            <asp:Label ID="UserIdLabel" runat="server" Text='<%# Eval("UserId") %>' Visible="false"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

            <%-- Notification Label --%>
            <div id="toastContainer" aria-live="polite" aria-atomic="true" style="position: relative; min-height: 200px;"></div>
        </div>
    </form>

    <%-- Script for Hide/Show Password --%>
    <script type="text/javascript">
        document.getElementById('togglePassword').addEventListener('click', function (e) {
            var passwordField = document.getElementById('<%= UserPass.ClientID %>');
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                e.target.classList.remove('fa-eye');
                e.target.classList.add('fa-eye-slash');
            } else {
                passwordField.type = 'password';
                e.target.classList.remove('fa-eye-slash');
                e.target.classList.add('fa-eye');
            }
        });

        document.getElementById('togglePassword1').addEventListener('click', function (e) {
            var passwordField = document.getElementById('<%= PassMod.ClientID %>');
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                e.target.classList.remove('fa-eye');
                e.target.classList.add('fa-eye-slash');
            } else {
                passwordField.type = 'password';
                e.target.classList.remove('fa-eye-slash');
                e.target.classList.add('fa-eye');
            }
        });
    </script>
</body>
</html>
