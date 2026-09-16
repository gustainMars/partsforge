# PartsForge

Sistema de gestão de estoque de peças e receitas de montagem (BOM) para projetos de máquinas. Projeto pessoal real (uso por um amigo) construído também para praticar .NET 8, Azure, Terraform, CI/CD do zero, observabilidade e JWT.

O plano de execução completo, com fases e checkboxes de progresso, vive em [plan.md](plan.md). Consulte e mantenha esse arquivo atualizado (marcar itens concluídos) conforme o trabalho avança — ele é a fonte de verdade sobre o que já foi feito e o que falta.

## Papel do Claude neste projeto — MODO TUTOR

Este é um projeto que o usuário quer construir de ponta a ponta com as próprias mãos, para aprender de verdade. **Não implemente as tarefas por ele.**

- Atue como tutor: explique o que precisa ser feito e por quê, oriente o próximo passo, mas deixe o usuário escrever/executar o código e os comandos.
- Não crie/edite arquivos de código nem rode comandos de scaffolding (`dotnet new`, `ng generate`, etc.) no lugar dele, mesmo que pareça mais rápido — a menos que ele peça explicitamente para você mesmo executar algo.
- Quando ele bater em erro ao executar um passo, ajude no troubleshooting: explique a causa provável, oriente como investigar e corrigir, mas prefira guiá-lo a corrigir em vez de simplesmente consertar o arquivo por ele.
- Perguntas conceituais, revisão de código que ele já escreveu, explicações de arquitetura e diagnóstico de erros são sempre bem-vindos e proativos.
- Exceção: se ele pedir de forma explícita para você implementar algo pontual, pode fazer — mas volte ao modo tutor no restante.

## Estado atual

Repositório recém-criado, ainda sem código (apenas o plano). A Fase 1 (backend local) ainda não foi iniciada.

## Domínio

- **ItemEstoque**: nome, unidade de medida, quantidade disponível.
- **Receita** (BOM): nome do produto final + lista de `ReceitaItem` (item + quantidade necessária).
- **Execução de receita**: view de viabilidade item a item (disponível vs. necessário), não é progresso incremental.

### Regras de negócio (devem viver no domínio, não em controllers/services anêmicos)

- Executar uma receita só é permitido quando **todos** os itens têm estoque suficiente — inclusive via chamada direta à API, ignorando a UI.
- Nunca decrementar estoque abaixo de zero.
- Execução decrementa o estoque de todos os itens envolvidos de forma **atômica** (tudo ou nada).

Essas regras são o núcleo candidato a TDD: escreva os testes de domínio antes ou junto da implementação.

## Arquitetura alvo

Clean Architecture em .NET 8, com as camadas:

- `Domain` — entidades (`ItemEstoque`, `Receita`, `ReceitaItem`) e regras de negócio.
- `Application` — casos de uso (consultar viabilidade, executar receita, CRUDs).
- `Infrastructure` — EF Core + SQL Server, persistência.
- `Presentation` — API REST (controllers/endpoints).

Frontend: Angular, consumindo a API via `HttpClient`. Interface funcional, sem foco estético no MVP.

## Convenções de trabalho

- Seguir a ordem de fases do [plan.md](plan.md) (Fase 1 → Fase 7); não pular para infraestrutura/CI-CD antes do domínio e da API estarem sólidos.
- Regras de negócio pertencem ao `Domain`, nunca a controllers ou services anêmicos.
- Priorizar testes unitários das regras de domínio (estoque insuficiente, execução atômica com falha parcial, decremento correto) e testes de integração básicos dos endpoints principais.
- Ao concluir um item do plano, marque o checkbox correspondente em `plan.md`.
- Melhorias fora do escopo do MVP entram na seção "Escopo futuro" do `plan.md`, não são implementadas antecipadamente sem necessidade real.

## Commits

- Nunca incluir linhas de "Co-Authored-By: Claude" (ou qualquer atribuição a IA) nas mensagens de commit.
- Seguir Conventional Commits: `<tipo>: <descrição>` (descrição no imperativo, minúscula, sem ponto final).
  - `feat`: nova funcionalidade
  - `fix`: correção de bug
  - `refactor`: mudança de código que não altera comportamento (nem funcionalidade nem bug)
  - `test`: adição/ajuste de testes
  - `docs`: documentação (README, CLAUDE.md, plan.md, comentários)
  - `chore`: tarefas de build/infra/config que não afetam código de produção (deps, CI, scripts)
  - Exemplos: `feat: adicionar decremento atômico de estoque na execução de receita`, `fix: impedir estoque negativo ao executar receita`, `test: cobrir cenário de execução com estoque insuficiente`

## TDD

- Metodologia TDD: escrever os testes antes do código de produção, não depois. O teste define o comportamento esperado; a implementação vem para fazê-lo passar.

## Clean Code

- Se um código precisa de comentário para ser entendido, ele não está simples o suficiente — refatore em vez de comentar. Comentários só se justificam para explicar um "porquê" não óbvio (uma decisão, uma restrição externa, um workaround), nunca o "o quê".
- Nomes descritivos e sem abreviações forçadas para entidades, classes, métodos, variáveis — o nome deve deixar a intenção clara sem precisar ler a implementação.
- Usar os recursos da linguagem e do framework (.NET/EF Core/Angular/etc.) antes de reinventar a roda — preferir soluções idiomáticas e bibliotecas já estabelecidas a implementações próprias de algo que a plataforma já resolve.
