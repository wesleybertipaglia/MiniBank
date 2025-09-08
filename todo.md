# TODO - Planejamento do Projeto MiniBank

## Fase 1 – Estrutura Inicial

> Criar a estrutura base dos projetos e configurar o ambiente de desenvolvimento.

* [x] Criar a solution principal `MiniBank.sln`
* [x] Criar o projeto `MiniBank.ApiGateway`
* [x] Criar os projetos `MiniBank.Auth`, `MiniBank.Bank`, `MiniBank.Mailer` organizados por camadas:
  * `.API`
  * `.Application`
  * `.Core`
  * `.Infrastructure`
  * `.Tests`

* [x] Adicionar e configurar o Docker Compose com:
  * PostgreSQL
  * RabbitMQ

* [x] Configurar o Swagger em cada API
* [x] Configurar o Ocelot no API Gateway
* [x] Criar arquivos `.dockerignore` e `.gitignore`
* [x] Criar README inicial com instruções básicas

## Fase 2 – Serviço de Autenticação (Auth)

> Criar e implementar o serviço de autenticação de usuários.

* [ ] Definir e implementar entidades (ex: `User`, `Role`)
* [ ] Criar DTOs e mapeamentos com AutoMapper
* [ ] Implementar serviços principais (cadastro, login, autenticação JWT)
* [ ] Implementar repositórios e persistência com EF Core + PostgreSQL
* [ ] Configurar comunicação assíncrona via RabbitMQ (ex: envio de evento "UserCreated")
* [ ] Capturar logs com Serilog
* [ ] Criar testes unitários com xUnit
* [ ] Criar Dockerfile para build do serviço
* [ ] Atualizar docker-compose com o serviço
* [ ] Documentar endpoints no Swagger

## Fase 3 – Serviço Bancário (Bank)

> Criar e implementar o serviço responsável pelas contas e transações.

* [ ] Definir entidades (ex: `Account`, `Transaction`)
* [ ] Criar DTOs e mapeamentos
* [ ] Implementar regras de negócio (criação de conta, transferência, extrato)
* [ ] Integrar com RabbitMQ para consumir eventos do Auth (ex: `UserCreated`)
* [ ] Publicar eventos de transação
* [ ] Capturar logs com Serilog
* [ ] Criar testes unitários
* [ ] Criar Dockerfile
* [ ] Documentar endpoints

## Fase 4 – Serviço de Notificação (Mailer)

> Criar e implementar o serviço para envio de notificações por e-mail.

* [ ] Criar entidades (ex: `EmailNotification`)
* [ ] Criar DTOs e serviços para envio simulado de e-mail
* [ ] Consumir eventos do Bank (ex: `TransactionCreated`)
* [ ] Capturar logs com Serilog
* [ ] Criar testes unitários
* [ ] Criar Dockerfile
* [ ] Documentar funcionamento e payload esperado dos eventos

## Fase 5 – CI/CD com GitHub Actions

> Automatizar o ciclo de build, testes e deploy.

* [ ] Criar workflow de build e testes:
  * Roda em PRs e commits no `main`
  * Executa `dotnet build`, `dotnet test`

* [ ] Criar workflow de build e push de imagens Docker
  * Tag automático por commit

* [ ] Configurar secrets do GitHub (ex: `DOCKER_HUB_USERNAME`, `DOCKER_HUB_TOKEN`)
* [ ] Criar step para deploy (caso tenha infraestrutura definida)

## Fase 6 – Deploy

> Subir o sistema em ambiente real ou simulado.

* [ ] Definir ambiente de destino (EC2, ECS, Azure, VPS, etc.)
* [ ] Criar arquivos de configuração de deploy
* [ ] Subir banco de dados e serviços com Docker Compose ou Kubernetes
* [ ] Expor API Gateway para acesso externo
* [ ] Validar funcionamento de ponta a ponta
* [ ] Documentar processo de deploy

## Fase 7 – Validação e Documentação

* [ ] Validar cenário completo de criação de usuário, abertura de conta e transação
* [ ] Garantir que eventos fluem entre serviços
* [ ] Garantir logs estão sendo capturados
* [ ] Atualizar README com instruções completas
* [ ] Criar diagrama da arquitetura (opcional)
