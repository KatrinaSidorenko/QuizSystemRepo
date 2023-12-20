using Core.DTO;
using Core.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace BLL.Services
{
    public class AttemptDocumentService : IDocument
    {
        private readonly AttemptResultDocumentDTO _documentModel;
        public AttemptDocumentService(AttemptResultDocumentDTO documentDTO)
        {
            _documentModel = documentDTO;
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
                    column.Item().Text($"Test title: {_documentModel.Name}").Style(titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span($"Questions amount: ").SemiBold();
                        text.Span($" {_documentModel.Questions.Count}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span($"Max mark: ").SemiBold();
                        text.Span($"{_documentModel.Questions.Sum(q => q.Point)}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span($"Geted points: ").SemiBold();
                        text.Span($"{_documentModel.Mark}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span($"Start Date: ").SemiBold();
                        text.Span($"{_documentModel.StartDate.ToString("dddd, dd MMMM yyyy HH:mm", CultureInfo.InvariantCulture)}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span($"End Date: ").SemiBold();
                        text.Span($"{_documentModel.EndDate.ToString("dddd, dd MMMM yyyy HH:mm", CultureInfo.InvariantCulture)}");
                    });
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            var rightAnswerStyle = TextStyle.Default.SemiBold().FontColor(Colors.Green.Medium);
            var wrongAnswerStyle = TextStyle.Default.SemiBold().FontColor(Colors.Red.Medium);

            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(10);

                for (int i = 1; i <= _documentModel.Questions.Count; i++)
                {
                    var question = _documentModel.Questions[i - 1];
                 
                    column.Item().Column(questionColumn =>
                    {
                        questionColumn.Item().Text($"{i}. (Type: {question.Type}). Question: {question.Description}     max({question.Point})").Bold();
                    });

                        column.Item().Column(answerColumn =>
                        {
                            for (int j = 1; j <= question.Answers.Count; j++)
                            {
                                var answer = question.Answers[j - 1];

                                if (question.Type.Equals(QuestionType.Open))
                                {
                                    if (answer.Value.ToLower().Equals(answer.ValueByUser.ToLower()))
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"Right answer: ");
                                            row.RelativeItem().Text($"{answer.Value}").Style(rightAnswerStyle);
                                        });
                                    }
                                    else if(answer.IsRight && !answer.ChoosenByUser)
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"Right answer: ");
                                            row.RelativeItem().Text($"{answer.Value}").Style(rightAnswerStyle);
                                        });
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"Your answer: ");
                                            row.RelativeItem().Text($"{answer.ValueByUser}").Style(wrongAnswerStyle);
                                        });
                                    }
                                    else
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"Answer Text: ");
                                            row.RelativeItem().Text($"{answer.ValueByUser}");
                                        });
                                    }
                                }
                                else
                                {
                                    if(answer.IsRight && answer.ChoosenByUser)
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"({j}) + ");
                                            row.RelativeItem().Text($"{answer.Value}").Style(rightAnswerStyle);
                                        });
                                    }
                                    else if(answer.IsRight && !answer.ChoosenByUser)
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"{j}) ");
                                            row.RelativeItem().Text($"{answer.Value}").Style(rightAnswerStyle);
                                        });
                                    }
                                    else if(!answer.IsRight && answer.ChoosenByUser)
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"({j}) ");
                                            row.RelativeItem().Text($"{answer.Value}").Style(wrongAnswerStyle);
                                        });
                                    }
                                    else
                                    {
                                        answerColumn.Item().PaddingLeft(5).Row(row =>
                                        {
                                            row.Spacing(5);
                                            row.AutoItem().Text($"{j}) ");
                                            row.RelativeItem().Text($"{answer.Value}");
                                        });
                                    }
                                }
                            }
                        });
                }
            });
        }
    }
}
