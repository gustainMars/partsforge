# PartsForge

API de gestão de estoque de peças e receitas de montagem (BOM). Uma receita lista os itens de estoque necessários para montar um produto. A API compara, item a item, o que há em estoque com o que a receita exige e, quando tudo está disponível, executa a receita baixando o estoque de forma atômica (tudo ou nada).

> Projeto em desenvolvimento, feito para estudo. O roteiro completo está em [plan.md](plan.md).

## Tecnologias

.NET 8 · ASP.NET Core · Clean Architecture · CQRS com MediatR · EF Core 8 · SQL Server 2022 (Docker) · xUnit e Moq

## Arquitetura

| Camada | Responsabilidade |
| --- | --- |
| `Domain` | Entidades e regras de negócio (estoque nunca negativo, execução atômica, itens sem duplicidade) |
| `Application` | Casos de uso (consultar viabilidade, executar receita) e contratos de repositório |
| `Infrastructure` | EF Core, mapeamentos, migrations e implementação dos repositórios |
| `Presentation` | API REST, DTOs de resposta e tradução de exceções para HTTP |

As dependências apontam para dentro: `Presentation` e `Infrastructure` dependem de `Application`, que depende de `Domain`.

## Rodando localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/products/docker-desktop/)
- Ferramenta do EF Core: `dotnet tool install --global dotnet-ef --version 8.0.11`

### Passo a passo

1. **Configure a senha do banco.** Copie `.env.example` para `.env` e defina `MSSQL_SA_PASSWORD` (mínimo de 8 caracteres, com maiúscula, minúscula, número e símbolo, exigência do SQL Server).

2. **Suba o SQL Server:**

   ```bash
   docker compose up -d
   ```

   Ao subir, você aceita o EULA do SQL Server (edição Developer, apenas para desenvolvimento e testes).

3. **Informe a connection string à API** via User Secrets, usando a mesma senha do `.env`:

   ```bash
   dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Database=PartsForge;User Id=sa;Password=SUA_SENHA;TrustServerCertificate=True;" --project src/PartsForge.Presentation
   ```

4. **Crie as tabelas.** O comando usa a API como projeto de inicialização, então lê a mesma connection string do passo anterior:

   ```bash
   dotnet ef database update --project src/PartsForge.Infrastructure --startup-project src/PartsForge.Presentation
   ```

5. **Carregue dados de exemplo** (opcional, mas necessário para experimentar os endpoints):

   ```bash
   docker cp scripts/seed-dev.sql partsforge-sqlserver:/tmp/seed-dev.sql
   docker exec partsforge-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "SUA_SENHA" -C -b -d PartsForge -i /tmp/seed-dev.sql
   ```

6. **Rode a API:**

   ```bash
   dotnet run --project src/PartsForge.Presentation --launch-profile http
   ```

   O Swagger abre em http://localhost:5015/swagger.

Para parar o banco: `docker compose down` (mantém os dados) ou `docker compose down -v` (apaga tudo).

## Endpoints

| Método | Rota | Respostas |
| --- | --- | --- |
| `GET` | `/api/receitas/{receitaId}/viabilidade` | `200` com a comparação item a item · `404` |
| `POST` | `/api/receitas/{receitaId}/executar` | `204` · `404` · `422` quando algum item não tem estoque suficiente |

Com os dados de exemplo, a receita `1` (Motor de Prensa Hidráulica) está inviável e a `2` (Kit de Fixação) está viável. Resposta de `GET /api/receitas/1/viabilidade`:

```json
[
  { "itemEstoqueId": 1, "descricao": "Motor 220V", "necessario": 2, "disponivel": 1, "suficiente": false, "disponibilidade": "1/2" },
  { "itemEstoqueId": 2, "descricao": "Parafuso M6", "necessario": 12, "disponivel": 12, "suficiente": true, "disponibilidade": "12/12" },
  { "itemEstoqueId": 3, "descricao": "Porca M6", "necessario": 12, "disponivel": 11, "suficiente": false, "disponibilidade": "11/12" }
]
```

O `422` de `executar` devolve a mesma lista no campo `itens`, então quem chama a API já recebe o panorama completo sem uma segunda requisição.

## Testes

```bash
dotnet test
```

Os testes atuais são unitários (Domain e Application) e não precisam do banco.

## Licença

Código-fonte disponível sob a [PolyForm Noncommercial License 1.0.0](LICENSE) (`PolyForm-Noncommercial-1.0.0`). Você pode ler, rodar, estudar e modificar o projeto para fins não comerciais. Uso comercial, como vender ou oferecer o sistema como produto ou serviço, não é permitido sem autorização do autor.

**Permissão adicional:** o autor autoriza expressamente que recrutadores e avaliadores técnicos baixem e executem o projeto para avaliar o trabalho em processos seletivos.

Isso não é uma licença "open source" no sentido da OSI, justamente por restringir o uso comercial.
