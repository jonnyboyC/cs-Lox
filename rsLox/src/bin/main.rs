extern crate rs_lox;

use rs_lox::scanning::token::{Literal, Token};
use rs_lox::scanning::token_type::TokenType;

fn main() {
    let a = Token::new(
        TokenType::String, 
        "herp".to_string(), 
        Some(Literal::String("herp".to_string())), 
        5
    );

    println!("{}", a.unwrap());

    println!("Hello, world!");
}
