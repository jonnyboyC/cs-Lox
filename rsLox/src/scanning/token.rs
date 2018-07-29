use scanning::token_type::{TokenType, LiteralTokenType};
use scanning::scanning_error::ScanningError;
use std::fmt;

#[derive(Debug)]
pub enum Literal {
    Number(f64),
    String(String)
}

#[derive(Debug)]
pub struct Token {
  pub token_type: TokenType,
  pub lexeme: String,
  pub literal: Option<Literal>,
  pub line: u32
}


impl Token {
  pub fn new (
    token_type: TokenType, 
    lexeme: String, 
    literal: Option<Literal>,
    line: u32) -> Result<Token, ScanningError>
  {
    // sanity check that we don't accidently pass a literal when we shouldn't
    match token_type.is_literal() {
      LiteralTokenType::Literal => {
        match literal {
          Some(_) => Ok(Token { token_type, lexeme, literal, line }),
          None => Err(ScanningError::TokenLiteralFound(token_type))
        } 
      }
      LiteralTokenType::Identifier => {
        match literal {
          Some(_) => Err(ScanningError::TokenLiteralFound(token_type)),
          None => Ok(Token { token_type, lexeme, literal, line })
        }
      }
    }
  }
}

impl fmt::Display for Token {
  fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
    match self.literal
    {
      Some(ref literal) => match literal {
        Literal::Number(number) => write!(f, "{} {} {}", self.token_type, self.lexeme, number),
        Literal::String(string) => write!(f, "{} {} {}", self.token_type, self.lexeme, string),
      },
      None => write!(f, "{} {}", self.token_type, self.lexeme)
    }
  }
}



#[cfg(test)]
mod tests {
  use super::*;

  #[test]
  fn vec2_constructor() {
    let token_type = TokenType::SemiColon;
    let lexeme = "test";
    let literal = Some(Literal::String("test"));
    let line = 10;

    match Token::New(token_type, lexeme, literal, line) {
      Ok(token) => {
        assert_eq!(line, token.line);
        assert_eq!(lexeme, token.lexeme);
        match token.literal {
          Some(lit) => assert_eq!(literal, lit),
          None => assert!(false) 
        }
        assert_eq!(token_type, token.token_type);
      },
      Err(err) => {
        assert!(false)
      }
    }
  }
}

