-- Dados de exemplo para desenvolvimento local. Idempotente: nao faz nada se ja existirem receitas.
IF NOT EXISTS (SELECT 1 FROM Receitas)
BEGIN
    INSERT INTO ItensEstoque (Descricao, Quantidade) VALUES
        ('Motor 220V', 1), ('Parafuso M6', 12), ('Porca M6', 11);

    INSERT INTO Receitas (Descricao) VALUES
        ('Motor de Prensa Hidraulica'), ('Kit de Fixacao');

    DECLARE @motor INT = (SELECT Id FROM ItensEstoque WHERE Descricao = 'Motor 220V');
    DECLARE @parafuso INT = (SELECT Id FROM ItensEstoque WHERE Descricao = 'Parafuso M6');
    DECLARE @porca INT = (SELECT Id FROM ItensEstoque WHERE Descricao = 'Porca M6');
    DECLARE @prensa INT = (SELECT Id FROM Receitas WHERE Descricao = 'Motor de Prensa Hidraulica');
    DECLARE @kit INT = (SELECT Id FROM Receitas WHERE Descricao = 'Kit de Fixacao');

    INSERT INTO ReceitaItens (ReceitaId, ItemEstoqueId, Quantidade) VALUES
        (@prensa, @motor, 2), (@prensa, @parafuso, 12), (@prensa, @porca, 12),
        (@kit, @parafuso, 4), (@kit, @porca, 4);
END
