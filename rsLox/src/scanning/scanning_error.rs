use std::fmt;
use scanning::token_type::TokenType;

#[derive(Debug)]
pub enum ScanningError {
  TokenLiteralFound(TokenType),
  TokenLiteralNotFound(TokenType)
}

impl fmt::Display for ScanningError {
  fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
    match self {
      ScanningError::TokenLiteralFound(token_type) => {
        write!(f, "literal found for tokentype {}.", token_type)
      }
      ScanningError::TokenLiteralNotFound(token_type) => {
        write!(f, "literal not found for tokentype {}.", token_type)
      }
    }
  }
}