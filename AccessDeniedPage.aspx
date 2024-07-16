<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AccessDeniedPage.aspx.cs" Inherits="ClaimPortalUserCreation.AccessDeniedPage" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Access Denied</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.9.0/css/bootstrap-datepicker.min.css" />

    <style>
        body {
            background-color: white;
        }

        .access-denied-container {
            text-align: center;
            margin-top: 100px;
        }

        .access-denied-text {
            font-size: 24px;
            font-weight: bold;
            color: #333; /* Dark gray */
        }

        .padlock-icon {
            font-size: 48px;
            color: #333;
        }

        .additional-text {
            font-size: 16px;
            color: #666;
        }

        .form-control {
            text-align: center;
        }

        @media (max-width: 768px) {
            .col-12 {
                text-align: center;
            }

            #btnOpenModal {
                width: 100%;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container body-content">
            <div class="access-denied-container">
                <div class="access-denied-text">ACCESS DENIED</div>
                <div class="padlock-icon">🔒</div>
                <div class="additional-text">
                    YOU DON'T HAVE ACCESS TO VIEW THIS PAGE. IF YOU SHOULD BE ABLE TO ACCESS THIS PAGE, PLEASE CONTACT OUR TEAM.
                </div>
            </div>
        </div>
    </form>
</body>
</html>
