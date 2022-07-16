//GAME CONTROL CONTRACT

// SPDX-License-Identifier: MIT
pragma solidity ^0.4.26;


interface IERC20 {
    function totalSupply() external view returns (uint);
    function balanceOf(address account) external view returns (uint);
    function transfer(address recipient, uint amount) external returns (bool);
    function allowance(address owner, address spender) external view returns (uint);
    function approve(address spender, uint amount) external returns (bool);
    function transferFrom(address sender, address recipient, uint256 amount) external returns (bool);
    event Transfer(address indexed from, address indexed to, uint value);
    event Approval(address indexed owner, address indexed spender, uint value);
}

contract GameControl {

    uint256 private balance;
    
    mapping(address => bool) public active;
    address tokens = 0x549e80263f03334eB4Af74497F8B5D3B7Fda2BFB;

    
    function receiveTokens(uint256 _amount) external payable{    

        IERC20(tokens).transfer(address(msg.sender), _amount);
        active[msg.sender] = true;

    }

    event Received(address, uint);

    function () payable public {
        emit Received(msg.sender, msg.value);
    }

     function getBalanceBNB() public view returns(uint) {
        return address(this).balance;
    }
}