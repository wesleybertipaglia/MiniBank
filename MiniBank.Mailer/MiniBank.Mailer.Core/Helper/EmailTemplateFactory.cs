using MiniBank.Mailer.Core.Dto;

namespace MiniBank.Mailer.Core.Helper;

public static class EmailTemplateFactory
{
    public static (string Subject, string Body) BuildConfirmationEmail(UserDto user)
    {
        var subject = "Confirmação de E-mail - MiniBank";

        var confirmationUrl = $"https://localhost:5020/api/user/confirm-email/{user.Id}";

        var body = $"""
                    Olá, {user.Name}!

                    Obrigado por se registrar no MiniBank. Para concluir seu cadastro, por favor confirme seu e-mail clicando no link abaixo:

                    {confirmationUrl}

                    Se você não solicitou este cadastro, ignore este e-mail.

                    Atenciosamente,  
                    Equipe MiniBank
                    """;

        return (subject, body);
    }

    public static (string Subject, string Body) BuildWelcomeEmail(UserDto user)
    {
        var subject = "Bem-vindo ao MiniBank 🎉";

        var body = $"""
                    Olá, {user.Name}!

                    Sua conta no MiniBank foi criada com sucesso e já está pronta para uso.

                    Agora você pode:
                    - Realizar transferências
                    - Consultar seu saldo
                    - Receber depósitos
                    - E muito mais!

                    Acesse o sistema e explore os recursos disponíveis.

                    Se tiver qualquer dúvida, entre em contato com nosso suporte.

                    Seja bem-vindo(a) e bons negócios!

                    Atenciosamente,  
                    Equipe MiniBank
                    """;

        return (subject, body);
    }
}