# PartsForge — Plano de Execução (Escopo Inicial)

> Sistema de gestão de estoque de peças e receitas de montagem para projetos de máquinas. Este documento cobre o **escopo inicial (MVP)** — melhorias futuras devem ser adicionadas como novas fases ao final deste plano, mantendo o histórico do que já foi decidido.

## Objetivo
Construir um sistema real (para um amigo) de controle de estoque de peças e "receitas" de montagem de produtos, aproveitando o projeto para preencher lacunas técnicas identificadas em processos seletivos recentes: **.NET 10, Azure, Terraform, CI/CD criado do zero, observabilidade, JWT**.

## Contexto de negócio (levantamento com o usuário final)

O colega validou a ideia com o usuário final (dono de uma fábrica de máquinas) e fez um levantamento de requisitos mais aprofundado. Alguns pontos mudam o centro de gravidade do projeto:

- O objetivo do usuário final é controlar estoque, cadastrar a lista de materiais (BOM) de cada máquina, saber **o que comprar e quando** (MRP simplificado), e **centralizar o conhecimento técnico** (desenhos e sequência de montagem) para a empresa não depender de uma única pessoa.
- Existe uma lacuna de mercado: soluções de MRP completas são caras e voltadas a empresas grandes; as ferramentas baratas não fazem esse tipo de gestão. O público-alvo é empresas industriais de pequeno/médio porte (≈20-30 funcionários) que hoje fazem tudo manualmente (planilhas, e-mail).
- Isso reforça que **MRP e anexos de engenharia são o núcleo de valor do produto**, à frente dos itens de infraestrutura que haviam sido priorizados antes. As fases abaixo foram reordenadas para refletir isso (ver "Ordem de prioridade sugerida").
- Um modelo de negócio por assinatura (SaaS) foi cogitado como direção futura, mas é uma decisão de produto, não de escopo técnico do MVP — não deve travar decisões de modelagem, mas vale evitar acoplamentos que impeçam multiusuário/multiempresa mais adiante.

## Domínio do sistema

- **Item de Estoque**: código interno (digitado pelo usuário na criação, único, separado do Id técnico — permite localizar e excluir o item pelo código), descrição, unidade de medida, quantidade disponível, tipo (**comprado** ou **fabricado por terceiros**), lead time em dias, desenho técnico anexado (PDF/DXF).
- **Fornecedor**: razão social, CNPJ, telefone, nome do contato (uma empresa pode ter mais de um fornecedor cadastrado, e um fornecedor pode ter mais de uma pessoa de contato/vendedor).
- **Item de Fornecedor** (associação N:N entre Item de Estoque e Fornecedor): código do produto no fornecedor, lead time específico daquele fornecedor para aquele item. Ao lançar uma ordem de compra com um código de fornecedor ainda não mapeado, o sistema deve permitir associá-lo a um item interno já existente.
- **Histórico de Compra**: item, fornecedor, valor e data da compra — alimentado pela entrada da nota fiscal.
- **Receita (BOM - Bill of Materials)**: nome do produto final + lista de itens necessários com quantidade.
- **Necessidade de Compra (MRP)**: dado um produto (receita/código), uma quantidade desejada (N ≥ 1) e uma data de início de montagem, o sistema calcula por item: disponível em estoque, necessário total, o que falta comprar, e a data limite de compra (data de início − lead time do item). Tem **modo simulação** (consulta, não altera nada) e **modo execução** (dá baixa real no estoque). É a evolução direta da consulta de viabilidade já implementada.
- **Ordem de Compra**: numeração sequencial própria, dados do fornecedor, itens, valores; gerada em PDF a partir de uma necessidade de compra identificada pelo MRP.
- **Desenho Técnico**: anexo (PDF/DXF) vinculado a um Item de Estoque (desenho da peça, prioridade para itens fabricados por terceiros) ou a uma Receita (desenho de montagem + sequência/roteiro de montagem passo a passo).
- **Perfil de Acesso**: usuário multiusuário com permissões distintas — hoje só operacional, mas pensado para comportar futuramente perfis de financeiro e vendas.
- **Tela de Execução (viabilidade/MRP)**: exibe a comparação por item — quantidade disponível vs. necessária. O botão "Executar" fica **desabilitado** se qualquer item tiver estoque insuficiente para compra imediata, e a necessidade de compra é gerada para os itens faltantes.

### Regras de negócio candidatas (boas para TDD)

- A execução só pode ser habilitada quando **todos** os itens da receita têm estoque suficiente (regra já implementada para a execução simples de receita).
- Não é possível executar uma receita/MRP com estoque insuficiente em qualquer item, mesmo via chamada direta à API, ignorando a UI.
- Não é possível decrementar estoque abaixo de zero.
- Executar uma receita ou uma execução de MRP decrementa o estoque de todos os itens envolvidos de forma **atômica** (tudo ou nada).
- A geração de necessidades (MRP) é feita por produto (código) + quantidade desejada (N ≥ 1) + data de início de montagem.
- `Receita.VerificarViabilidade` e `Receita.Executar` ganham um parâmetro `quantidadeDesejada` com valor padrão 1 (compatível com o comportamento atual, sem quebrar chamadas existentes). `Necessario` passa a ser `ReceitaItem.Quantidade * quantidadeDesejada`, e o decremento em `Executar` precisa usar o mesmo produto — as duas contas têm que mudar juntas, senão a viabilidade considera `N` mas a baixa de estoque não, gerando consumo real menor do que o calculado sem nenhum erro aparente. Vale um teste específico para essa sincronização (`Executar(quantidadeDesejada: 3)` decrementando exatamente `Quantidade * 3`).
- A data limite de compra de cada item = data de início da montagem − lead time do item (ou do fornecedor selecionado para aquele item, quando houver mais de um cadastrado).
- O MRP considera apenas o **estoque físico atual** — não desconta pedidos de compra já em andamento; a resposta a isso é justamente gerar a ordem de compra, não reservar estoque virtual.
- **Modo simulação** do MRP não altera nada no sistema; **modo execução** decrementa o estoque de todos os itens envolvidos de forma atômica, igual à execução de receita já implementada.
- Para o MVP, cada execução de MRP (simulação ou execução) é tratada isoladamente — não há alocação automática de estoque compartilhado entre execuções simultâneas de máquinas diferentes que usam o mesmo item (o próprio usuário reconheceu o risco de conflito e preferiu simplificar por ora; ver "Escopo futuro").
- Um item de estoque pode estar associado a múltiplos fornecedores, cada um com seu próprio código de produto e lead time.
- A entrada de nota fiscal de compra dá entrada (incremento) automático no estoque do(s) item(ns) recebido(s) e registra o histórico de compra (valor, data, fornecedor).

---

## Fase 1 — Backend local (Clean Architecture)
**Meta:** domínio sólido, testável, rodando localmente.

- [x] Criar solução .NET 10 com camadas: `Domain`, `Application`, `Infrastructure`, `Presentation`
- [x] Atualizado de .NET 8 para .NET 10 (LTS) em 2026-09-30 — o suporte do .NET 8 encerra em 10/11/2026 (só patches de segurança desde então); upgrade feito cedo, enquanto o projeto ainda é pequeno e o custo de mudança é baixo
- [x] Modelar entidades de domínio: `ItemEstoque`, `Receita`, `ReceitaItem`
- [x] Implementar regras de negócio no domínio (não em controllers/services anêmicos)
- [x] Configurar Entity Framework Core + SQL Server (via Docker local)
- [ ] Endpoints REST:
  - CRUD de itens de estoque
  - CRUD de receitas (com itens associados)
  [x] Consultar viabilidade de uma receita (disponível vs. necessário, por item)
  [x] Executar uma receita (decrementa estoque de todos os itens, de forma atômica, se viável)
- [x] Testes unitários das regras de domínio (casos: execução com estoque insuficiente, execução atômica com falha parcial, decremento correto ao executar)
- [ ] Testes de integração básicos nos endpoints principais

## Fase 2 — MRP: geração de necessidades de compra
**Meta:** evoluir a consulta de viabilidade atual para o cálculo de MRP descrito pelo usuário final — o núcleo de valor do produto.

- [ ] Adicionar campo de lead time (dias) ao cadastro de `ItemEstoque`
- [ ] Adicionar campo de tipo do item (comprado / fabricado por terceiros)
- [ ] Estender `Receita.VerificarViabilidade(int quantidadeDesejada = 1)` e `Receita.Executar(int quantidadeDesejada = 1)` para multiplicar `Necessario` e o decremento por `quantidadeDesejada`, com validação de `N ≥ 1` (exceção de domínio própria) e atenção a overflow de `int` na multiplicação
- [ ] Caso de uso: dado um código de receita, uma quantidade desejada (N) e uma data de início de montagem, calcular por item: disponível, necessário total (considerando N), falta comprar, data limite de compra (usa o lead time genérico do `ItemEstoque` — ver decisão sobre lead time por fornecedor)
- [ ] Modo simulação (somente consulta, não altera estoque)
- [ ] Modo execução (dá baixa real no estoque, atômica, igual à execução de receita atual)
- [ ] Testes unitários: quantidade N > 1 (viabilidade **e** decremento sincronizados), N inválido (< 1), cálculo de data limite por item, atomicidade da execução, diferença entre modo simulação e execução

## Fase 3 — Fornecedores e itens de fornecedor
**Meta:** permitir associar cada item de estoque a um ou mais fornecedores.

- [ ] Cadastro de `Fornecedor` (razão social, CNPJ, telefone, contato)
- [ ] Associação N:N `Item de Estoque` ↔ `Fornecedor`, com código do produto no fornecedor e lead time específico daquele fornecedor
- [ ] Fluxo de associação ao lançar uma ordem de compra com código de fornecedor ainda não mapeado (buscar/selecionar o item interno correspondente)
- [ ] Testes cobrindo: item com múltiplos fornecedores, lead time por fornecedor prevalecendo sobre o lead time genérico do item (a definir)

## Fase 4 — Compras: nota fiscal e ordem de compra
**Meta:** fechar o ciclo "o que comprar → comprei → chegou".

- [ ] Entrada de nota fiscal de compra: dá baixa (incremento) automático no estoque e registra histórico de compra (valor, data, fornecedor)
- [ ] Histórico de compras por item (aba no cadastro do item)
- [ ] Geração de ordem de compra em PDF a partir de uma necessidade identificada pelo MRP, com numeração sequencial própria (modelo de referência: ordem de compra anexada pelo usuário — não precisa ser idêntica, mas cobrir os mesmos dados essenciais: fornecedor, itens, valores, número da OC)
- [ ] Testes cobrindo: entrada de nota fiscal incrementa estoque corretamente, numeração sequencial de OC não se repete

## Fase 5 — Engenharia: desenhos e roteiro de montagem
**Meta:** centralizar o conhecimento técnico hoje mantido em pastas manuais.

- [ ] Upload de desenho técnico (PDF/DXF) vinculado a um item de estoque, priorizando itens fabricados por terceiros
- [ ] Disponibilizar desenho ao fornecedor (inicialmente apenas download/anexar; envio automático por e-mail fica para escopo futuro)
- [ ] Upload de desenho de montagem vinculado a uma receita
- [ ] Registro da sequência de montagem (passo a passo) vinculado a uma receita
- [ ] Testes cobrindo validações de upload (formato, tamanho) e associação correta ao item/receita

## Fase 6 — Frontend básico (Angular)
**Meta:** interface funcional, não precisa ser bonita.

- [ ] Tela de cadastro/listagem de itens de estoque (com lead time e tipo)
- [ ] Tela de criação de receita (selecionar itens + quantidades)
- [ ] Tela de MRP: informar produto, quantidade e data de início; exibir por item "disponível/necessário" e data limite de compra; alternar entre simulação e execução
- [ ] Tela de cadastro de fornecedores e associação de itens de fornecedor
- [ ] Consumo da API via serviços Angular (HttpClient)

## Fase 7 — Containerização
**Meta:** tudo rodando via Docker Compose localmente.

- [ ] Dockerfile multi-stage para a API (.NET 10)
- [ ] `docker-compose.yml` com API + banco de dados
- [ ] Validar que o ambiente sobe do zero com um único comando

## Fase 8 — Autenticação e perfis de acesso (JWT)
**Meta:** cobrir gap de autenticação que apareceu em várias vagas — e já é uma necessidade real confirmada pelo usuário final (multiusuário, com perfis futuros de financeiro e vendas).

- [ ] Implementar autenticação simples com JWT (login com usuário/senha, emissão de token)
- [ ] Modelar perfis de acesso (ao menos um perfil operacional inicial, preparando terreno para perfis futuros)
- [ ] Proteger endpoints sensíveis com `[Authorize]`

## Fase 9 — Infraestrutura como código (Terraform + Azure)
**Meta:** provisionar a infraestrutura real na nuvem, entendendo cada peça.

- [ ] Criar conta Azure (free tier / créditos, se disponível)
- [ ] Escrever módulos Terraform para provisionar:
  - Resource Group
  - Azure Database (SQL ou MySQL, compatível com o EF Core já usado)
  - Azure App Service (hospedar a API)
  - Azure Key Vault (armazenar connection string e secrets)
  - Azure Service Bus (fila para processamento assíncrono — ver Fase 11)
  - (Opcional) Application Insights, para observabilidade
- [ ] Rodar `terraform plan` e `terraform apply` manualmente primeiro — entender o que cada recurso faz antes de automatizar
- [ ] Documentar as decisões de infraestrutura (por que cada recurso, trade-offs)

## Fase 10 — CI/CD (criado do zero, não apenas utilizado)
**Meta:** pipeline real, de sua autoria, não apenas "vi rodar".

- [ ] Workflow no GitHub Actions:
  1. Build e testes automatizados do backend
  2. Build da imagem Docker
  3. Deploy no Azure App Service
- [ ] (Opcional, se sobrar tempo/energia) Explorar deploy do Terraform também via pipeline (Terraform Cloud ou GitHub Actions)

## Fase 11 — Processamento assíncrono com mensageria (Azure Service Bus)
**Meta:** cobrir o gap técnico mais recorrente entre as vagas analisadas (mensageria, processamento assíncrono, idempotência) — presente em Prezensa (RabbitMQ), nstech (ServiceBus) e, de forma central, na Harmo (filas, retries, idempotência, degradação parcial).

- [ ] Redesenhar o fluxo de "Executar MRP" para ser assíncrono:
  - Ao clicar em "Executar", a API publica uma mensagem na fila (Azure Service Bus) em vez de decrementar o estoque de forma síncrona.
  - Um worker (background service / consumer) processa a mensagem e realiza o decremento atômico do estoque.
  - O status da execução passa a ter estados: `Solicitada` → `Processando` → `Concluída` / `Falhou`.
- [ ] Implementar **idempotência** no consumidor: se a mesma mensagem for reprocessada (reentrega da fila), o resultado não pode ser aplicado em duplicidade (ex: usar um identificador único de execução e registrar o que já foi processado).
- [ ] Tratar **retries e dead-letter queue**: definir política de novas tentativas em caso de falha transitória, e mover para fila de mensagens mortas após N tentativas malsucedidas.
- [ ] Considerar (e documentar) o cenário de **degradação parcial**: o que acontece se o worker estiver indisponível — a solicitação de execução deve continuar sendo aceita (enfileirada) mesmo que o processamento fique temporariamente atrasado.
- [ ] Testes automatizados cobrindo: mensagem duplicada não gera decremento duplicado, falha simulada aciona retry, exaustão de tentativas move para dead-letter.
- [ ] Documentar no README as decisões de consistência eventual (por que assíncrono aqui, trade-offs assumidos).

## Fase 12 — Polimento para portfólio
**Meta:** projeto pronto para ser mostrado em entrevistas e no LinkedIn.

- [ ] README completo: visão geral, arquitetura, decisões técnicas, como rodar localmente
- [ ] Link de demo funcionando (se o Azure App Service estiver ativo)
- [ ] Repositório público no GitHub
- [ ] Post no LinkedIn contando o processo e os aprendizados (Azure, Terraform, CI/CD do zero, mensageria)

---

## Ordem de prioridade sugerida

Dado que há processos seletivos ativos em paralelo (Jane, BTG, Asaas), e que o levantamento com o usuário final deixou claro que MRP e engenharia são o núcleo de valor do produto, a prioridade é:

1. **Fase 1** (já bem avançada) e **Fase 2 (MRP)** primeiro — é o coração do produto segundo o próprio usuário final, e a evolução natural do que já existe.
2. **Fases 3 e 4** (fornecedores, nota fiscal e ordem de compra) na sequência — fecham o ciclo "o que comprar → comprei → chegou" em cima do MRP.
3. **Fase 5** (engenharia/desenhos) pode andar em paralelo às fases 3-4, por ser mais independente, mas não deve ficar para trás — também foi citada como central pelo usuário.
4. **Fases 6-7** (frontend, containerização) seguem fluência natural (Clean Architecture, Angular, Docker), sem pressa de prazo.
5. **Fase 8** (autenticação/perfis) sobe de prioridade em relação ao plano anterior, por já ter sido validada como necessidade real (multiusuário), mas pode vir depois do core funcional.
6. **Fases 9-11** (Terraform/Azure, CI/CD do zero, mensageria) continuam sendo objetivo de aprendizado — sem pressa de prazo, é onde vale errar e aprender de verdade — mas agora vêm depois do core de negócio validado com o usuário final.
7. **Fase 12** ao final, quando o projeto já estiver estável.

## Notas
- Priorizar sempre entrevistas e testes técnicos dos processos em andamento sobre o avanço deste projeto.
- Cada fase concluída já é, isoladamente, um bom tópico de conversa em entrevista — não é necessário terminar tudo para começar a usar o projeto como exemplo.

## Decisões em aberto

- **Substituir um item faltante em uma receita existente**: além de `AdicionarItem`, pode fazer sentido trocar/substituir um `ReceitaItem` já cadastrado (ex: item descontinuado por outro equivalente). Ainda não decidido como isso deve funcionar — confirmar com o usuário antes de implementar.
- **Teto de quantidade por item**: hoje `ItemEstoque.QuantidadeMaxima = int.MaxValue` (neutro). Confirmar com o usuário se existe um limite de negócio menor. Se sim, reduzir a constante, validar também no construtor e avaliar migração de dados existentes.
- **Código interno do Item de Estoque**: será digitado pelo usuário, não gerado pelo sistema. O formato (prefixo/sufixo, padronização por categoria etc.) ainda não foi definido — confirmar com o usuário final o padrão real usado por ele antes de desenhar a regra de validação (hoje só sabemos que precisa ser obrigatório e único).
- **Lead time por fornecedor vs. lead time genérico do item**: quando um item tem mais de um fornecedor cadastrado, o MRP deve usar o lead time de qual fornecedor (o mais rápido? o último usado? um fornecedor "preferencial" marcado no cadastro)? **Resolvido para a Fase 2**: o MRP nasce usando só o lead time genérico do `ItemEstoque` (Fase 3 ainda não existe nesse ponto do roteiro); ao implementar a Fase 3, o cálculo é estendido para considerar o lead time por fornecedor quando houver um cadastrado — mudança aditiva (lookup a mais na Application), não uma reescrita do que a Fase 2 entregar. A pergunta em aberto de verdade é só a regra de qual fornecedor prevalece, e essa sim fica para decidir quando a Fase 3 começar.
- **Numeração da ordem de compra**: sequencial global do sistema ou por empresa/ano? A definir na Fase 4.
- **Formato exato da ordem de compra gerada**: usar como inspiração o modelo enviado pelo usuário (ordem de compra real de uma empresa em que trabalhou), sem necessidade de ser idêntico — confirmar campos obrigatórios (negociação, forma de pagamento, frete, impostos) antes de fechar o layout na Fase 4.

## Escopo futuro (fora do MVP)
Ideias de melhorias a considerar após o escopo inicial estar estável — detalhar em fases próprias quando chegar a hora:
- [ ] Alocação inteligente de estoque compartilhado entre múltiplas execuções de MRP simultâneas que usam o mesmo item, priorizando por data limite mais próxima (cenário descrito pelo usuário: dois ou três clientes com pedidos e datas diferentes ao mesmo tempo).
- [ ] Suporte a múltiplas execuções da mesma receita com quantidades e datas de início diferentes por pedido (ex: 5 prensas da mesma receita, mas com datas limite diferentes por lote).
- [ ] Envio automático de ordem de compra e desenhos técnicos ao fornecedor por e-mail (hoje previsto apenas como download/anexo manual).
- [ ] Integração via XML de nota fiscal, caso passe a existir uma fonte estruturada (hoje a entrada é manual, sem XML disponível).
- [ ] Modelo multiempresa (SaaS) — cada empresa com seu próprio catálogo de itens/receitas, garantindo isolamento de dados entre clientes. Direção de produto cogitada, mas não é requisito técnico do MVP.
- [ ] (demais itens a definir conforme necessidades reais de uso do seu amigo)
