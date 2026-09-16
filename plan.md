# PartsForge — Plano de Execução (Escopo Inicial)

> Sistema de gestão de estoque de peças e receitas de montagem para projetos de máquinas. Este documento cobre o **escopo inicial (MVP)** — melhorias futuras devem ser adicionadas como novas fases ao final deste plano, mantendo o histórico do que já foi decidido.

## Objetivo
Construir um sistema real (para um amigo) de controle de estoque de peças e "receitas" de montagem de produtos, aproveitando o projeto para preencher lacunas técnicas identificadas em processos seletivos recentes: **.NET 8, Azure, Terraform, CI/CD criado do zero, observabilidade, JWT**.

## Domínio do sistema

- **Item de Estoque**: nome, unidade de medida, quantidade disponível (ex: "Parafuso M6", un, 34)
- **Receita (BOM - Bill of Materials)**: nome do produto final + lista de itens necessários com quantidade (ex: "Motor de Prensa Hidráulica" → 2x Motor 220V, 12x Parafuso, 12x Porca)
- **Tela de Execução**: para uma receita selecionada, exibe uma comparação por item — quantidade disponível em estoque vs. quantidade necessária (ex: "Motor 220V: 1/2", "Parafuso: 12/12", "Porca: 11/12"). É uma visão de viabilidade, não um progresso incremental de montagem.
  - O botão "Executar" fica **desabilitado** se qualquer item tiver estoque insuficiente, e **habilitado** somente quando todos os itens atendem à quantidade necessária.
  - Ao clicar em "Executar" (com todos os itens disponíveis), o sistema decrementa do estoque a quantidade total exigida pela receita, de uma vez.

Regras de negócio candidatas (boas para TDD):
- A execução só pode ser habilitada quando **todos** os itens da receita têm estoque suficiente.
- Não é possível executar uma receita com estoque insuficiente em qualquer item (mesmo que via chamada direta à API, ignorando a UI).
- Não é possível decrementar estoque abaixo de zero.
- Executar uma receita decrementa o estoque de todos os itens envolvidos, de forma atômica (tudo ou nada — se falhar em um item, nenhum é decrementado).

---

## Fase 1 — Backend local (Clean Architecture)
**Meta:** domínio sólido, testável, rodando localmente.

- [x] Criar solução .NET 8 com camadas: `Domain`, `Application`, `Infrastructure`, `Presentation`
- [x] Modelar entidades de domínio: `ItemEstoque`, `Receita`, `ReceitaItem`
- [x] Implementar regras de negócio no domínio (não em controllers/services anêmicos)
- [ ] Configurar Entity Framework Core + SQL Server (via Docker local)
- [ ] Endpoints REST:
  - CRUD de itens de estoque
  - CRUD de receitas (com itens associados)
  - Consultar viabilidade de uma receita (disponível vs. necessário, por item)
  - Executar uma receita (decrementa estoque de todos os itens, de forma atômica, se viável)
- [x] Testes unitários das regras de domínio (casos: execução com estoque insuficiente, execução atômica com falha parcial, decremento correto ao executar)
- [ ] Testes de integração básicos nos endpoints principais

## Fase 2 — Frontend básico (Angular)
**Meta:** interface funcional, não precisa ser bonita.

- [ ] Tela de cadastro/listagem de itens de estoque
- [ ] Tela de criação de receita (selecionar itens + quantidades)
- [ ] Tela de execução: para a receita selecionada, exibir por item "disponível/necessário" (ex: "1/2", "12/12", "11/12") e o botão "Executar" habilitado/desabilitado conforme viabilidade
- [ ] Consumo da API via serviços Angular (HttpClient)

## Fase 3 — Containerização
**Meta:** tudo rodando via Docker Compose localmente.

- [ ] Dockerfile multi-stage para a API (.NET 8)
- [ ] `docker-compose.yml` com API + banco de dados
- [ ] Validar que o ambiente sobe do zero com um único comando

## Fase 4 — Autenticação (JWT)
**Meta:** cobrir gap de autenticação que apareceu em várias vagas.

- [ ] Implementar autenticação simples com JWT (login com usuário/senha, emissão de token)
- [ ] Proteger endpoints sensíveis com `[Authorize]`

## Fase 5 — Infraestrutura como código (Terraform + Azure)
**Meta:** provisionar a infraestrutura real na nuvem, entendendo cada peça.

- [ ] Criar conta Azure (free tier / créditos, se disponível)
- [ ] Escrever módulos Terraform para provisionar:
  - Resource Group
  - Azure Database (SQL ou MySQL, compatível com o EF Core já usado)
  - Azure App Service (hospedar a API)
  - Azure Key Vault (armazenar connection string e secrets)
  - (Opcional) Application Insights, para observabilidade
- [ ] Rodar `terraform plan` e `terraform apply` manualmente primeiro — entender o que cada recurso faz antes de automatizar
- [ ] Documentar as decisões de infraestrutura (por que cada recurso, trade-offs)

## Fase 6 — CI/CD (criado do zero, não apenas utilizado)
**Meta:** pipeline real, de sua autoria, não apenas "vi rodar".

- [ ] Workflow no GitHub Actions:
  1. Build e testes automatizados do backend
  2. Build da imagem Docker
  3. Deploy no Azure App Service
- [ ] (Opcional, se sobrar tempo/energia) Explorar deploy do Terraform também via pipeline (Terraform Cloud ou GitHub Actions)

## Fase 7 — Polimento para portfólio
**Meta:** projeto pronto para ser mostrado em entrevistas e no LinkedIn.

- [ ] README completo: visão geral, arquitetura, decisões técnicas, como rodar localmente
- [ ] Link de demo funcionando (se o Azure App Service estiver ativo)
- [ ] Repositório público no GitHub
- [ ] Post no LinkedIn contando o processo e os aprendizados (Azure, Terraform, CI/CD do zero)

---

## Ordem de prioridade sugerida

Dado que há processos seletivos ativos em paralelo (Jane, BTG, Asaas), a prioridade é:

1. **Fases 1–3** primeiro — são fluência natural (Clean Architecture, Angular, Docker), rápidas de entregar.
2. **Fases 4–6** são o verdadeiro objetivo de aprendizado (JWT, Terraform, Azure, CI/CD do zero) — sem pressa de prazo, é onde vale errar e aprender de verdade.
3. **Fase 7** ao final, quando o projeto já estiver estável.

## Notas
- Priorizar sempre entrevistas e testes técnicos dos processos em andamento sobre o avanço deste projeto.
- Cada fase concluída já é, isoladamente, um bom tópico de conversa em entrevista — não é necessário terminar tudo para começar a usar o projeto como exemplo.

## Escopo futuro (fora do MVP)
Ideias de melhorias a considerar após o escopo inicial estar estável — detalhar em fases próprias quando chegar a hora:
- [ ] (a definir conforme necessidades reais de uso do seu amigo)

