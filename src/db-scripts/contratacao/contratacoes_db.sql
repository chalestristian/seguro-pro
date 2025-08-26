CREATE TABLE IF NOT EXISTS public.contratacoes(
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    proposta_id UUID NOT NULL,
    data_contratacao TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE IF NOT EXISTS public.propostas_elegiveis (
    proposta_id UUID PRIMARY KEY,
    aprovada_em TIMESTAMP WITH TIME ZONE NOT NULL
);