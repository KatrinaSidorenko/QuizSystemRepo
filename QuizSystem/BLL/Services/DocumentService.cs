using Core.DocumentModels;
using Core.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BLL.Services
{
    public class DocumentService : IDocument
    {
        private readonly TestDocumentModel _memberViewModel;
        public DocumentService(TestDocumentModel memberViewModel)
        {
            _memberViewModel = memberViewModel;   
        }
        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().ShowOnce().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);


                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        private void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.AlignLeft().Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Test title: {_memberViewModel.Name}").Style(titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span($"Description: ").SemiBold();
                        text.Span($" {_memberViewModel.Description}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span($"Questions amount: ").SemiBold();
                        text.Span($" {_memberViewModel.QuestionsAmount}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span($"Max mark: ").SemiBold();
                        text.Span($"{_memberViewModel.MaxMark}");
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {

            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(10);

                for (int i = 1; i <= _memberViewModel.Questions.Count; i++)
                {
                    var question = _memberViewModel.Questions[i - 1];
                    // Question Header
                    column.Item().Column(questionColumn =>
                    {
                        questionColumn.Item().Text($"{i}. (Type: {question.Type}). Question: {question.Description}").Bold();
                    });

                    if (question.Type.Equals(QuestionType.Open))
                    {
                        column.Item().Column(answerColumn =>
                        {
                            answerColumn.Spacing(5);
                            answerColumn.Item().PaddingLeft(5).Row(row =>
                            {
                                row.AutoItem().Text($"Your answer: ");
                            });

                            answerColumn.Item().PaddingBottom(25);
                        });
                    }
                    else
                    {
                        // Answer Options
                        column.Item().Column(answerColumn =>
                        {
                            for (int j = 1; j <= question.Answers.Count; j++)
                            {
                                var answer = question.Answers[j - 1];
                                answerColumn.Item().PaddingLeft(5).Row(row =>
                                {
                                    row.Spacing(5);
                                    row.AutoItem().Text($"{j}) ");
                                    row.RelativeItem().Text($"{answer.Value}");
                                });
                            }
                        });
                    }
                    
                }
            });
        }
    }
}
