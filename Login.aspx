<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="ClaimPortalUserCreation.Login" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Synapse Login</title>
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <link href="https://fonts.googleapis.com/css2?family=Noto+Sans+Gunjala+Gondi:wght@400..700&display=swap" rel="stylesheet">

    <style>
        .toast-custom {
            position: fixed;
            top: 10px;
            right: 20px;
            width: 300px;
            background-color: #fff;
            border-radius: 5px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
            padding: 10px;
        }

        .toast-success {
            border-left: 5px solid #28a745; /* Light green */
        }

        .toast-danger {
            border-left: 5px solid #dc3545; /* Red */
        }

        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Poppins', sans-serif;
        }

        body {
            /*background: #f0f2f5;*/
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            flex-direction: column;
        }

        .container {
            display: flex;
            align-items: center;
            justify-content: center;
            flex-direction: row;
            width: 80%;
            max-width: 1000px;
        }

        .left-section {
            flex: 1;
            display: flex;
            align-items: center; /* Align image & text in same row */
            gap: 30px; /* Adds spacing between image & text */
            margin-right: 50px;
        }

            .left-section img {
                width: 250px; /* Adjust size as needed */
                height: auto;
            }

            .left-section h1 {
                color: #1877f2;
                font-size: 55px;
                margin: 0;
                background: linear-gradient(90deg, #ff4f8b, #ff8743, #ffc107, #00bcd4, #3f51b5);
                -webkit-background-clip: text;
                -webkit-text-fill-color: transparent;
                display: inline-block;
                font-family: "Noto Sans Gunjala Gondi", sans-serif;
                font-optical-sizing: auto;
                font-weight: bold;
                font-style: normal;
            }

        .right-section {
            flex: 1;
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0px 2px 10px rgba(0, 0, 0, 0.2);
            width: 100%;
            max-width: 400px;
            text-align: center;
        }

        .input-group {
            margin-bottom: 15px;
            text-align: left;
        }

            .input-group input {
                width: 100%;
                padding: 12px;
                border: 1px solid #ddd;
                border-radius: 6px;
                font-size: 16px;
            }

        .btn-login {
            width: 100%;
            background: #1877f2;
            color: white;
            padding: 12px;
            border: none;
            border-radius: 6px;
            font-size: 18px;
            cursor: pointer;
            font-weight: bold;
            transition: 0.3s;
        }

            .btn-login:hover {
                background: #165cbb;
            }

        @media (max-width: 768px) {
            .container {
                flex-direction: column;
                text-align: center;
            }

            .left-section {
                margin-bottom: 20px;
                margin-right: 0;
                /*flex-direction: column;*/ /* Stack on smaller screens */
                text-align: center;
            }

                .left-section img {
                    width: 150px; /* Adjust size as needed */
                    height: auto;
                }

                .left-section h1 {
                    color: #1877f2;
                    font-size: 35px;
                    font-weight: bold;
                    margin: 0;
                    font-family: "Noto Sans Gunjala Gondi", sans-serif;
                    font-optical-sizing: auto;
                    font-style: normal;
                }
        }
    </style>

    <script src="https://code.jquery.com/jquery-3.6.4.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.bundle.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js"></script>

    <!-- Include CryptoJS -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/crypto-js/4.1.1/crypto-js.min.js"></script>

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

    <script>
        function encryptPassword() {
            var pwdField = document.getElementById('<%= Password.ClientID %>');

            // 32 chars = 256-bit key, 16 chars = 128-bit IV
            var key = CryptoJS.enc.Utf8.parse("MySuperSecretKeyForAES256_123456");
            var iv = CryptoJS.enc.Utf8.parse("MyInitVector1234");

            var encrypted = CryptoJS.AES.encrypt(pwdField.value, key, {
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

            pwdField.value = encodeURIComponent(encrypted.toString());
            return true;
        }
    </script>

</head>
<body>

    <div class="container">
        <div class="left-section">
            <img src="https://www.wipro.com/content/dam/wipro/social-icons/wipro_new_logo.svg" alt="Logo">
            <h1>SYNAPSE</h1>
        </div>

        <div class="right-section">
            <form id="form1" runat="server">
                <div class="input-group">
                    <%--<input type="text" placeholder="Email address or phone number" required>--%>
                    <asp:TextBox ID="UserId" runat="server" CssClass="form-control" placeholder="User Id"></asp:TextBox>
                </div>
                <div class="input-group">
                    <asp:TextBox ID="Password" Type="Password" runat="server" CssClass="form-control" placeholder="Password"></asp:TextBox>
                </div>
                <asp:Button ID="SubmitBtn" runat="server" CssClass="form-control btn-primary" Text="Log In" OnClientClick="return encryptPassword();" OnClick="SubmitBtn_Click" />

                <%-- Notification Label --%>
                <div id="toastContainer" aria-live="polite" aria-atomic="true"></div>
            </form>
        </div>
    </div>



</body>
</html>


