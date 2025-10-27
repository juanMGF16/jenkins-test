namespace Utilities.Templates
{
    /// <summary>
    /// Plantillas HTML para emails del sistema con estilos responsivos
    /// </summary>
    public static class EmailTemplates
    {
        //private static readonly string LogoBase64Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo_base64.txt");

        //private static string GetLogoHtml()
        //{
        //    if (!File.Exists(LogoBase64Path)) return "";
        //    string base64Logo = File.ReadAllText(LogoBase64Path);
        //    return $"<img src='data:image/png;base64,{base64Logo}' alt='Logo' style='max-width: 130px; margin-bottom: 5px;' />";
        //}

        /// <summary>
        /// Genera plantilla HTML para email de recuperación de contraseña
        /// </summary>
        /// <param name="username">Nombre de usuario destinatario</param>
        /// <param name="recoveryLink">Enlace de recuperación con token</param>
        /// <param name="expirationHours">Horas hasta que expire el enlace (24 por defecto)</param>
        /// <returns>HTML formateado del email</returns>
        public static string GetPasswordRecoveryTemplate(string username, string recoveryLink, int expirationHours = 24)
        {
            //string logoHtml = GetLogoHtml();

            return $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Recuperación de Contraseña</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            overflow: hidden;
                            box-shadow: 0 0 20px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            color: white;
                            padding: 15px 20px;
                            text-align: center;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 28px;
                            font-weight: 600;
                        }}
                        .content {{
                            padding: 40px 30px;
                            background-color: #f8f9fa;
                        }}
                        .greeting {{
                            font-size: 18px;
                            color: #2c3e50;
                            margin-bottom: 20px;
                        }}
                        .message {{
                            font-size: 16px;
                            color: #34495e;
                            margin-bottom: 25px;
                            line-height: 1.8;
                        }}
                        .button-container {{
                            text-align: center;
                            margin: 30px 0;
                        }}
                        .button {{
                            background: linear-gradient(135deg, rgb(30, 64, 175), rgb(26, 31, 54));
                            color: white;
                            padding: 16px 32px;
                            text-decoration: none;
                            border-radius: 50px;
                            display: inline-block;
                            font-weight: 600;
                            font-size: 16px;
                            text-transform: uppercase;
                            letter-spacing: 0.5px;
                            box-shadow: 0 4px 15px rgba(30, 64, 175, 0.3);
                            transition: all 0.3s ease;
                        }}
                        .button:hover {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            transform: translateY(-2px);
                            box-shadow: 0 6px 20px rgba(30, 64, 175, 0.4);
                        }}
                        .footer {{
                            text-align: center;
                            padding: 25px;
                            background-color: #2c3e50;
                            color: #ecf0f1;
                            font-size: 12px;
                        }}
                        .footer a {{
                            color: #3498db;
                            text-decoration: none;
                        }}
                        .expiration {{
                            background-color: #e8f5e8;
                            border: 1px solid #28a745;
                            border-radius: 5px;
                            padding: 12px;
                            margin: 15px 0;
                            text-align: center;
                            font-size: 14px;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                margin: 10px;
                                border-radius: 8px;
                            }}
                            .content {{
                                padding: 20px 15px;
                            }}
                            .button {{
                                padding: 14px 28px;
                                font-size: 14px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <img src='https://i.ibb.co/VcS9v8ZH/Logo.png' alt='Logo' style='max-width: 130px; margin-bottom: 5px;' />
                            <h1>🔐 Recuperación de Contraseña</h1>
                        </div>
                        
                        <div class='content'>
                            <div class='greeting'>
                                Hola <strong>{username}</strong>,
                            </div>
                            
                            <div class='message'>
                                Hemos recibido una solicitud para restablecer la contraseña de tu cuenta. 
                                Si no realizaste esta solicitud, por favor ignora este mensaje.
                            </div>
                
                            <div class='button-container'>
                                <a href='{recoveryLink}' class='button' target='_blank'
                                    class='button'
                                    style='color: #ffffff !important; text-decoration: none; display: inline-block;'>
                                    Restablecer Contraseña
                                </a>
                            </div>
                
                            <div class='expiration'>
                                ⏰ <strong>Este enlace expirará en {expirationHours} horas</strong>
                            </div>
                
                            <div class='message'>
                                Si tienes problemas para hacer clic en el botón, copia y pega la siguiente URL en tu navegador:
                                <br>
                                <code style='word-break: break-all; color: #007bff;'>{recoveryLink}</code>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} <strong>Codexy</strong>.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// Genera plantilla HTML de bienvenida simple al sistema
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="loginLink">Enlace para iniciar sesión</param>
        /// <returns>HTML formateado del email</returns>
        public static string GetWelcomeTemplate(string username, string loginLink)
        {
            //string logoHtml = GetLogoHtml();

            return $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Bienvenida</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            overflow: hidden;
                            box-shadow: 0 0 20px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            color: white;
                            padding: 15px 20px;
                            text-align: center;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 28px;
                            font-weight: 600;
                        }}
                        .content {{
                            padding: 40px 30px;
                            background-color: #f8f9fa;
                        }}
                        .greeting {{
                            font-size: 18px;
                            color: #2c3e50;
                            margin-bottom: 20px;
                        }}
                        .message {{
                            font-size: 16px;
                            color: #34495e;
                            margin-bottom: 25px;
                            line-height: 1.8;
                        }}
                        .button-container {{
                            text-align: center;
                            margin: 30px 0;
                        }}
                        .button {{
                            background: linear-gradient(135deg, rgb(30, 64, 175), rgb(26, 31, 54));
                            color: white;
                            padding: 16px 32px;
                            text-decoration: none;
                            border-radius: 50px;
                            display: inline-block;
                            font-weight: 600;
                            font-size: 16px;
                            text-transform: uppercase;
                            letter-spacing: 0.5px;
                            box-shadow: 0 4px 15px rgba(30, 64, 175, 0.3);
                            transition: all 0.3s ease;
                        }}
                        .button:hover {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            transform: translateY(-2px);
                            box-shadow: 0 6px 20px rgba(30, 64, 175, 0.4);
                        }}
                        .footer {{
                            text-align: center;
                            padding: 25px;
                            background-color: #2c3e50;
                            color: #ecf0f1;
                            font-size: 12px;
                        }}
                        .footer a {{
                            color: #3498db;
                            text-decoration: none;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                margin: 10px;
                                border-radius: 8px;
                            }}
                            .content {{
                                padding: 20px 15px;
                            }}
                            .button {{
                                padding: 14px 28px;
                                font-size: 14px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <img src='https://i.ibb.co/VcS9v8ZH/Logo.png' alt='Logo' style='max-width: 130px; margin-bottom: 5px;' />
                            <h1>🎉 ¡Bienvenido a nuestro sistema!</h1>
                        </div>
                        
                        <div class='content'>
                            <div class='greeting'>
                                Hola <strong>{username}</strong>,
                            </div>
                            
                            <div class='message'>
                                ¡Te damos la bienvenida a nuestra plataforma! Tu cuenta ha sido creada exitosamente.
                            </div>
                
                            <div class='button-container'>
                                <a href='{loginLink}' target='_blank'
                                    class='button'
                                    style='color: #ffffff !important; text-decoration: none; display: inline-block;'>
                                    Iniciar Sesión
                                </a>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} <strong>Codexy</strong></p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// Genera plantilla HTML de bienvenida con credenciales para nuevos usuarios
        /// </summary>
        /// <param name="name">Nombre completo del usuario</param>
        /// <param name="username">Nombre de usuario para acceso</param>
        /// <param name="password">Contraseña temporal</param>
        /// <param name="placeName">Nombre del lugar asignado (sucursal/zona)</param>
        /// <param name="roleName">Nombre del rol asignado</param>
        /// <param name="companyName">Nombre de la empresa</param>
        /// <returns>HTML formateado del email con credenciales</returns>
        public static string GetUserWelcomeTemplate(string name, string username, string password, string placeName, string roleName, string companyName)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Bienvenida {roleName}</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            overflow: hidden;
                            box-shadow: 0 0 20px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            color: white;
                            padding: 30px 20px;
                            text-align: center;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 28px;
                            font-weight: 600;
                        }}
                        .content {{
                            padding: 40px 30px;
                            background-color: #f8f9fa;
                        }}
                        .greeting {{
                            font-size: 18px;
                            color: #2c3e50;
                            margin-bottom: 20px;
                        }}
                        .message {{
                            font-size: 16px;
                            color: #34495e;
                            margin-bottom: 25px;
                            line-height: 1.8;
                        }}
                        .credentials {{
                            background-color: #e8f5e8;
                            border: 2px solid #28a745;
                            border-radius: 12px;
                            padding: 25px;
                            margin: 25px 0;
                            box-shadow: 0 4px 15px rgba(40, 167, 69, 0.15);
                        }}
                        .credentials h3 {{
                            margin-top: 0;
                            color: #155724;
                            font-size: 20px;
                            font-weight: 600;
                            text-align: center;
                            margin-bottom: 20px;
                        }}
                        .credential-item {{
                            margin: 15px 0;
                            font-size: 16px;
                            padding: 12px;
                            background-color: white;
                            border-radius: 8px;
                            border-left: 4px solid #28a745;
                            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                        }}
                        .credential-item strong {{
                            color: #155724;
                        }}
                        .warning {{
                            background-color: #fff3cd;
                            border: 2px solid #ffc107;
                            border-radius: 12px;
                            padding: 20px;
                            margin: 25px 0;
                            box-shadow: 0 4px 15px rgba(255, 193, 7, 0.15);
                        }}
                        .warning strong {{
                            color: #856404;
                        }}
                        .button-container {{
                            text-align: center;
                            margin: 30px 0;
                        }}
                        .button {{
                            background: linear-gradient(135deg, rgb(30, 64, 175), rgb(26, 31, 54));
                            color: white;
                            padding: 16px 32px;
                            text-decoration: none;
                            border-radius: 50px;
                            display: inline-block;
                            font-weight: 600;
                            font-size: 16px;
                            text-transform: uppercase;
                            letter-spacing: 0.5px;
                            box-shadow: 0 4px 15px rgba(30, 64, 175, 0.3);
                            transition: all 0.3s ease;
                        }}
                        .button:hover {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            transform: translateY(-2px);
                            box-shadow: 0 6px 20px rgba(30, 64, 175, 0.4);
                        }}
                        .footer {{
                            text-align: center;
                            padding: 25px;
                            background-color: #2c3e50;
                            color: #ecf0f1;
                            font-size: 12px;
                        }}
                        .footer p {{
                            margin: 5px 0;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                margin: 10px;
                                border-radius: 8px;
                            }}
                            .content {{
                                padding: 20px 15px;
                            }}
                            .header {{
                                padding: 20px 15px;
                            }}
                            .header h1 {{
                                font-size: 24px;
                            }}
                            .credentials {{
                                padding: 15px;
                            }}
                            .credential-item {{
                                font-size: 14px;
                                padding: 10px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <img src='https://i.ibb.co/VcS9v8ZH/Logo.png' alt='Logo' style='max-width: 130px; margin-bottom: 15px;' />
                            <h1>🎉 Bienvenido a {companyName}</h1>
                            <p style='margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Has sido asignado como {roleName} de {placeName}</p>
                        </div>
                        
                        <div class='content'>
                            <div class='greeting'>
                                Hola <strong>{name}</strong>,
                            </div>
                            
                            <div class='message'>
                                Te damos la bienvenida al sistema de gestión de sucursales. A continuación encontrarás tus credenciales de acceso:
                            </div>
                
                            <div class='credentials'>
                                <h3>🔐 Tus Credenciales de Acceso</h3>
                                <div class='credential-item'><strong>👤 Usuario:</strong> {username}</div>
                                <div class='credential-item'><strong>🔒 Contraseña:</strong> {password}</div>
                                <div class='credential-item'><strong>🏢 Sucursal:</strong> {placeName}</div>
                            </div>
                
                            <div class='warning'>
                                ⚠️ <strong>Importante de Seguridad:</strong><br>
                                • Cambia tu contraseña después del primer inicio de sesión<br>
                                • No compartas tus credenciales con nadie<br>
                            </div>

                            <div class='button-container'>
                                <a href='http://localhost:4200/Login' target='_blank'
                                    class='button'
                                    style='color: #ffffff !important; text-decoration: none; display: inline-block;'>
                                    Iniciar Sesión
                                </a>
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} <strong>Codexy</strong>.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// Genera plantilla HTML de bienvenida para usuarios operativos
        /// </summary>
        /// <param name="name">Nombre del operativo</param>
        /// <param name="lastname">Apellido del operativo</param>
        /// <returns>HTML formateado del email de bienvenida</returns>
        public static string GetWelcomeOperativeTemplate(string name, string lastname)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Bienvenida Usuario Operativo</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            overflow: hidden;
                            box-shadow: 0 0 20px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            color: white;
                            padding: 30px 20px;
                            text-align: center;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 28px;
                            font-weight: 600;
                        }}
                        .content {{
                            padding: 40px 30px;
                            background-color: #f8f9fa;
                        }}
                        .greeting {{
                            font-size: 20px;
                            color: #2c3e50;
                            margin-bottom: 25px;
                            text-align: center;
                            font-weight: 600;
                        }}
                        .message {{
                            font-size: 16px;
                            color: #34495e;
                            margin-bottom: 20px;
                            line-height: 1.8;
                            text-align: center;
                        }}
                        .highlight-box {{
                            background: linear-gradient(135deg, rgba(30, 64, 175, 0.1), rgba(26, 31, 54, 0.1));
                            border: 2px solid rgba(30, 64, 175, 0.3);
                            border-radius: 12px;
                            padding: 25px;
                            margin: 30px 0;
                            text-align: center;
                            box-shadow: 0 4px 15px rgba(30, 64, 175, 0.1);
                        }}
                        .highlight-box h3 {{
                            margin-top: 0;
                            color: #1e40af;
                            font-size: 18px;
                            font-weight: 600;
                            margin-bottom: 15px;
                        }}
                        .tasks-list {{
                            background-color: #e8f4f8;
                            border: 2px solid #0ea5e9;
                            border-radius: 12px;
                            padding: 20px;
                            margin: 25px 0;
                            box-shadow: 0 4px 15px rgba(14, 165, 233, 0.1);
                        }}
                        .tasks-list h3 {{
                            margin-top: 0;
                            color: #0369a1;
                            font-size: 18px;
                            font-weight: 600;
                            text-align: center;
                            margin-bottom: 15px;
                        }}
                        .task-item {{
                            margin: 12px 0;
                            padding: 10px 15px;
                            background-color: white;
                            border-radius: 8px;
                            border-left: 4px solid #0ea5e9;
                            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
                            font-size: 15px;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 25px;
                            background-color: #2c3e50;
                            color: #ecf0f1;
                            font-size: 12px;
                        }}
                        .footer p {{
                            margin: 5px 0;
                        }}
                        .welcome-icon {{
                            font-size: 48px;
                            margin-bottom: 15px;
                            display: block;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                margin: 10px;
                                border-radius: 8px;
                            }}
                            .content {{
                                padding: 20px 15px;
                            }}
                            .header {{
                                padding: 20px 15px;
                            }}
                            .header h1 {{
                                font-size: 24px;
                            }}
                            .greeting {{
                                font-size: 18px;
                            }}
                            .highlight-box, .tasks-list {{
                                padding: 15px;
                            }}
                            .task-item {{
                                font-size: 14px;
                                padding: 8px 12px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <img src='https://i.ibb.co/VcS9v8ZH/Logo.png' alt='Logo' style='max-width: 130px; margin-bottom: 15px;' />
                            <h1>🚀 ¡Bienvenido al Equipo!</h1>
                            <p style='margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Usuario Operativo</p>
                        </div>
                        
                        <div class='content'>
                            <div class='greeting'>
                                👋 Hola <strong>{name} {lastname}</strong>
                            </div>
                            
                            <div class='message'>
                                ¡Te damos la más cordial bienvenida a nuestra plataforma! Has sido asignado como <strong>Usuario Operativo</strong>.
                            </div>

                            <div class='highlight-box'>
                                <h3>🎯 Tu Rol en el Sistema</h3>
                                <p>Como usuario operativo, serás responsable de ejecutar el inventario en las debidas y mantener una comunicación constante con tu Encargado de Zona.</p>
                            </div>

                            <div class='tasks-list'>
                                <h3>📋 Próximos Pasos</h3>
                                <div class='task-item'>📅 Atento a tu horario de trabajo asignado</div>
                                <div class='task-item'>👥 Sigue las indicaciones de tu Encargado de Zona</div>
                                <div class='task-item'>✅ Completa el invenario asignado puntualmente</div>
                            </div>

                            <div class='message'>
                                Estamos seguros de que serás una pieza fundamental en nuestro equipo. 
                                <br>¡Mucho éxito en esta nueva etapa!
                            </div>
                        </div>
                        
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} <strong>Codexy</strong>.
                        </div>
                    </div>
                </body>
                </html>
            ";
        }

        /// <summary>
        /// Genera plantilla HTML para notificar asignación a un grupo operativo
        /// </summary>
        /// <param name="name">Nombre del operativo</param>
        /// <param name="lastname">Apellido del operativo</param>
        /// <param name="groupName">Nombre del grupo operativo</param>
        /// <param name="startDate">Fecha de inicio de asignación</param>
        /// <param name="endDate">Fecha de finalización de asignación</param>
        /// <returns>HTML formateado del email de asignación</returns>
        public static string GetOperativeAssignmentTemplate(string name, string lastname, string groupName, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1'>
                    <title>Asignación de Grupo Operativo</title>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            line-height: 1.6;
                            color: #333;
                            margin: 0;
                            padding: 0;
                            background-color: #f4f4f4;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            background-color: #ffffff;
                            border-radius: 10px;
                            overflow: hidden;
                            box-shadow: 0 0 20px rgba(0,0,0,0.1);
                        }}
                        .header {{
                            background: linear-gradient(135deg, rgb(26, 31, 54), rgb(30, 64, 175));
                            color: white;
                            padding: 30px 20px;
                            text-align: center;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 28px;
                            font-weight: 600;
                        }}
                        .content {{
                            padding: 40px 30px;
                            background-color: #f8f9fa;
                        }}
                        .greeting {{
                            font-size: 20px;
                            color: #2c3e50;
                            margin-bottom: 25px;
                            text-align: center;
                            font-weight: 600;
                        }}
                        .message {{
                            font-size: 16px;
                            color: #34495e;
                            margin-bottom: 20px;
                            line-height: 1.8;
                            text-align: center;
                        }}
                        .assignment-card {{
                            background: linear-gradient(135deg, rgba(16, 185, 129, 0.1), rgba(5, 150, 105, 0.1));
                            border: 2px solid rgba(16, 185, 129, 0.3);
                            border-radius: 12px;
                            padding: 30px;
                            margin: 30px 0;
                            text-align: center;
                            box-shadow: 0 4px 15px rgba(16, 185, 129, 0.1);
                        }}
                        .assignment-card h2 {{
                            margin-top: 0;
                            color: #065f46;
                            font-size: 22px;
                            font-weight: 600;
                            margin-bottom: 25px;
                        }}
                        .assignment-details {{
                            display: grid;
                            grid-template-columns: 1fr 1fr;
                            margin-bottom: 25px;
                        }}
                        .detail-item {{
                            grid-column: 1 / -1;
                            background-color: white;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0,0,0,0.08);
                            text-align: center;
                            margin-bottom: 20px;      
                        }}
                        .detail-label {{
                            font-size: 14px;
                            color: #6b7280;
                            margin-bottom: 8px;
                            font-weight: 500;
                        }}
                        .detail-value {{
                            font-size: 18px;
                            color: #065f46;
                            font-weight: 600;
                        }}
                        .date-range {{
                            grid-column: 1 / -1;
                            background: linear-gradient(135deg, #10b981, #059669);
                            color: white;
                            padding: 25px;
                            border-radius: 8px;
                            text-align: center;
                        }}
                        .date-range .detail-label {{
                            color: #d1fae5;
                            font-size: 16px;
                        }}
                        .date-range .detail-value {{
                            color: white;
                            font-size: 20px;
                        }}
                        .instructions {{
                            background-color: #e8f4f8;
                            border: 2px solid #0ea5e9;
                            border-radius: 12px;
                            padding: 25px;
                            margin: 25px 0;
                            box-shadow: 0 4px 15px rgba(14, 165, 233, 0.1);
                        }}
                        .instructions h3 {{
                            margin-top: 0;
                            color: #0369a1;
                            font-size: 18px;
                            font-weight: 600;
                            text-align: center;
                            margin-bottom: 20px;
                        }}
                        .instruction-item {{
                            margin: 15px 0;
                            padding: 12px 15px;
                            background-color: white;
                            border-radius: 8px;
                            border-left: 4px solid #0ea5e9;
                            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
                            font-size: 15px;
                            display: flex;
                            align-items: center;
                        }}
                        .instruction-icon {{
                            margin-right: 12px;
                            font-size: 18px;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 25px;
                            background-color: #2c3e50;
                            color: #ecf0f1;
                            font-size: 12px;
                        }}
                        .footer p {{
                            margin: 5px 0;
                        }}
                        @media (max-width: 600px) {{
                            .container {{
                                margin: 10px;
                                border-radius: 8px;
                            }}
                            .content {{
                                padding: 20px 15px;
                            }}
                            .header {{
                                padding: 20px 15px;
                            }}
                            .header h1 {{
                                font-size: 24px;
                            }}
                            .assignment-details {{
                                grid-template-columns: 1fr;
                                gap: 15px;
                            }}
                            .date-range {{
                                grid-column: 1;
                            }}
                            .detail-item {{
                                padding: 15px;
                            }}
                            .instructions {{
                                padding: 20px;
                            }}
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <img src='https://i.ibb.co/VcS9v8ZH/Logo.png' alt='Logo' style='max-width: 130px; margin-bottom: 15px;' />
                            <h1>📋 Asignación de Grupo</h1>
                            <p style='margin: 10px 0 0 0; font-size: 16px; opacity: 0.9;'>Nueva asignación operativa</p>
                        </div>
                        
                        <div class='content'>
                            <div class='greeting'>
                                👤 Hola <strong>{name} {lastname}</strong>
                            </div>
                            
                            <div class='message'>
                                Se te ha asignado un nuevo grupo operativo. A continuación encontrarás los detalles de tu asignación:
                            </div>

                            <div class='assignment-card'>
                                <h2>🎯 Detalles de la Asignación</h2>
                                
                                <div class='assignment-details'>
                                    <div class='detail-item'>
                                        <div class='detail-label'>Grupo Operativo</div>
                                        <div class='detail-value'>{groupName}</div>
                                    </div>
                                   
                                    
                                    <div class='date-range'>
                                        <div class='detail-label'>Período de Asignación</div>
                                        <div class='detail-value'>{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}</div>
                                    </div>
                                </div>
                                
                                <div style='font-size: 14px; color: #047857; margin-top: 15px;'>
                                    ⏰ Duración: {((endDate - startDate).TotalDays + 1):0} días
                                </div>
                            </div>

                            <div class='instructions'>
                                <h3>📝 Instrucciones Importantes</h3>
                                
                                <div class='instruction-item'>
                                    <span class='instruction-icon'>✅</span>
                                    Presentarte puntualmente en las fechas asignadas
                                </div>
                                
                                <div class='instruction-item'>
                                    <span class='instruction-icon'>👥</span>
                                    Coordinar con tu Encargado de Zona y compañeros del grupo
                                </div>
                                
                                <div class='instruction-item'>
                                    <span class='instruction-icon'>📱</span>
                                    Mantener comunicación constante durante el período
                                </div>
                            </div>
                        </div>
                        
                        <div class='footer'>
                           <p>© {DateTime.Now.Year} <strong>Codexy</strong>.</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
        }
    }
}
