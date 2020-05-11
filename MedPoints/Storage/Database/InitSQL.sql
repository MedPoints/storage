CREATE TABLE transactions(
  tx_hash VARCHAR(500) PRIMARY KEY,
  user_id VARCHAR(80) NOT NULl,
  type SMALLINT NOT NULl,
  transaction JSONB NOT NULL
);


CREATE TABLE blocks
(
  id SERIAL PRIMARY KEY,
  hash VARCHAR(500) PRIMARY KEY,
  data JSONB NOT NULL
);

CREATE TABLE mempool
(
  tx_hash VARCHAR(500) PRIMARY KEY,
  user_id VARCHAR(80) NOT NULl,
  type SMALLINT NOT NULl,
  transaction JSONB NOT NULL
);


