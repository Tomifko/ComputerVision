% Check distribution of times of our implementation
data = dlmread('OurJustTime.txt');
histogram(data(:,3))
xlabel('Time', 'FontSize', 15)
ylabel('Count', 'FontSize', 15)

% % Check distribution of times of built-in function
dataBuiltIn = dlmread('BuiltInJustTime.txt');
figure;
histogram(dataBuiltIn(:,3))
xlabel('Time', 'FontSize', 15)
ylabel('Count', 'FontSize', 15)

figure;
plot(data)
hold on;
plot(dataBuiltIn)
hold off
ylim([-0.025 0.4])

xlabel('Iteration', 'FontSize', 15)
ylabel('Time', 'FontSize', 15)


