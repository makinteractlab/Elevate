`default_nettype none

// define a Count module
module top(input wire [19:0] IN,
			  output wire OUT,
			  output wire LED
			  );	
	
	
	
	
	//OPNDRN i0 (.in(IN), .out(OUT[0]));
	wire result;
	assign result = 	IN[0] & 
							IN[1] & 
							IN[2] & 
							IN[3] & 
							IN[4] & 
							IN[5] & 
							IN[6] & 
							IN[7] & 
							IN[8] & 
							IN[9] & 
							IN[10] & 
							IN[11] & 
							IN[12] & 
							IN[13] & 
							IN[14] & 
							IN[15] & 
							IN[16] & 
							IN[17] & 
							IN[18] & 
							IN[19];
							
	assign LED= result;
	assign OUT = result;


endmodule
