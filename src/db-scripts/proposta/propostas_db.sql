CREATE TABLE IF NOT EXISTS public.propostas(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    nome_cliente VARCHAR(200) NOT NULL,
    cpf_cliente VARCHAR(11) NOT NULL,
    valor_seguro DECIMAL(18, 2) NOT NULL,
    status INT NOT NULL,
    data_criacao TIMESTAMP WITH TIME ZONE NOT NULL,
    data_atualizacao TIMESTAMP WITH TIME ZONE NOT NULL
);